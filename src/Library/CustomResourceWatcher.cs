using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using KubeClient.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public abstract class CustomResourceWatcher<TSpec> : ICustomResourceWatcher<TSpec>, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICustomResourceClient _client;
        private readonly string _crdPluralName;
        private readonly ConcurrentDictionary<string, TSpec> _resources = new ConcurrentDictionary<string, TSpec>();
        private IDisposable _subscription;

        protected CustomResourceWatcher(ILogger logger, ICustomResourceClient client, string crdPluralName)
        {
            _logger = logger;
            _client = client;
            _crdPluralName = crdPluralName;

            Subscribe();
        }

        public IEnumerable<TSpec> Resources => _resources.Values;

        private void Subscribe()
        {
            DisposeSubscriptions();
            _subscription = _client.Watch<TSpec>(_crdPluralName).Subscribe(OnNext, OnError);
        }

        private void OnNext(IResourceEventV1<CustomResource<TSpec>> @event)
        {
            switch (@event.EventType)
            {
                case ResourceEventType.Added:
                case ResourceEventType.Modified:
                    _resources.AddOrUpdate(@event.Resource.GlobalName, @event.Resource.Spec, (_, spec) => @event.Resource.Spec);
                    break;

                case ResourceEventType.Deleted:
                    _resources.TryRemove(@event.Resource.GlobalName, out _);
                    break;
            }
        }

        private void OnError(Exception exception)
        {
            _logger.LogError(exception, $"Error occured during watch for custom resource of type {typeof(TSpec).Name}.");
            Thread.Sleep(1000);
            Subscribe();
        }

        private void DisposeSubscriptions() => _subscription?.Dispose();

        public virtual void Dispose() => DisposeSubscriptions();
    }
}
