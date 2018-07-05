using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HTTPlease;
using KubeClient.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public abstract class CustomResourceWatcher<TSpec> : ICustomResourceWatcher<TSpec>, IDisposable
    {
        private const string RESOURCE_VERSION_NONE = "0";
        private readonly Dictionary<string, TSpec> _resources = new Dictionary<string, TSpec>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger _logger;
        private readonly ICustomResourceClient _client;
        private readonly string _apiGroup;
        private readonly string _crdPluralName;
        private IDisposable _subscription;
        private string _lastSeenResourceVersion = RESOURCE_VERSION_NONE;

        protected CustomResourceWatcher(ILogger logger, ICustomResourceClient client, string apiGroup, string crdPluralName)
        {
            _logger = logger;
            _client = client;
            _apiGroup = apiGroup;
            _crdPluralName = crdPluralName;
        }

        public IEnumerable<TSpec> Resources => new CrdMemento(_resources);
        public event EventHandler<Exception> OnConnectionError;
        public event EventHandler OnConnected;

        public void StartWatching()
        {
            if (_subscription == null)
                Subscribe();
        }

        private void Subscribe()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            DisposeSubscriptions();
            _subscription = _client.Watch<TSpec>(_apiGroup, _crdPluralName, _lastSeenResourceVersion).Subscribe(OnNext, OnError, OnCompleted);
            OnConnected?.Invoke(this, EventArgs.Empty);
            _logger.LogDebug($"Subscribed to {_crdPluralName}.");
        }

        private void OnNext(IResourceEventV1<CustomResource<TSpec>> @event)
        {
            _lastSeenResourceVersion = @event.Resource.Metadata.ResourceVersion;
            switch (@event.EventType)
            {
                case ResourceEventType.Added:
                case ResourceEventType.Modified:
                    _resources[@event.Resource.GlobalName] = @event.Resource.Spec;
                    break;
                case ResourceEventType.Deleted:
                    if (_resources.ContainsKey(@event.Resource.GlobalName))
                        _resources.Remove(@event.Resource.GlobalName);
                    break;
                case ResourceEventType.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _logger.LogInformation($"{@event.EventType} {@event.Resource.GlobalName}");
        }

        private void OnError(Exception exception)
        {
            _logger.LogError(exception, $"Error occured during watch for custom resource of type {typeof(TSpec).Name}. Resubscribing...");
            if (exception is HttpRequestException<StatusV1> requestException
             && requestException.StatusCode == HttpStatusCode.Gone)
            {
                _resources.Clear();
                _logger.LogDebug($"Cleaned resource cache for '{typeof(TSpec).Name}' as the last seen resource version ({_lastSeenResourceVersion}) is gone.");
                _lastSeenResourceVersion = RESOURCE_VERSION_NONE;
            }
            OnConnectionError?.Invoke(this, exception);
            Thread.Sleep(1000);
            Subscribe();
        }

        private void OnCompleted()
        {
            _logger.LogDebug($"Connection closed by Kube API during watch for custom resource of type {typeof(TSpec).Name}. Resubscribing...");
            OnConnectionError?.Invoke(this, new OperationCanceledException());
            Thread.Sleep(1000);
            Subscribe();
        }

        private void DisposeSubscriptions()
        {
            _subscription?.Dispose();
            _subscription = null;
            _logger.LogDebug($"Unsubscribed from {_crdPluralName}.");
        }

        public virtual void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            DisposeSubscriptions();
        }

        private class CrdMemento : IEnumerable<TSpec>
        {
            private readonly Dictionary<string, TSpec> _toIterate;

            public CrdMemento(Dictionary<string, TSpec> toIterate) => _toIterate = toIterate;

            public IEnumerator<TSpec> GetEnumerator() => _toIterate.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
