using System;
using System.Net;
using System.Reactive.Subjects;
using FluentAssertions;
using HTTPlease;
using IdentityServer4.Models;
using KubeClient.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class CustomResourceWatcherFacts
    {
        private readonly TestResourceWatcher _watcher;
        private readonly Subject<IResourceEventV1<CustomResource<Client>>> _resourceSubject;
        private readonly Mock<ICustomResourceClient> _resourceClientMock;

        public CustomResourceWatcherFacts()
        {
            _resourceSubject = new Subject<IResourceEventV1<CustomResource<Client>>>();
            _resourceClientMock = new Mock<ICustomResourceClient>();
            _resourceClientMock.SetupSequence(mock => mock.Watch<Client>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(_resourceSubject)
                               .Returns(new Subject<IResourceEventV1<CustomResource<Client>>>());
            _watcher = new TestResourceWatcher(_resourceClientMock.Object);
            _watcher.StartWatching();
        }

        [Fact]
        public void AddedResourceGetsAddedToCache()
        {
            var expectedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");

            _resourceSubject.OnNext(expectedResource);

            _watcher.Resources.Should().Contain(expectedResource.Resource.Spec);
        }

        [Fact]
        public void ModifiedResourceGetsUpdatedInCache()
        {
            var addedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");
            var modifiedResource = CreateResourceEvent(ResourceEventType.Modified, "expectedClientId");
            modifiedResource.Resource.Spec.ClientName = "clientname";
            _resourceSubject.OnNext(addedResource);

            _resourceSubject.OnNext(modifiedResource);

            _watcher.Resources.Should().Contain(modifiedResource.Resource.Spec);
        }

        [Fact]
        public void DeletedResourceGetsRemovedFromCache()
        {
            var watcherResources = _watcher.Resources;

            var addedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");
            var removedResource = CreateResourceEvent(ResourceEventType.Deleted, "expectedClientId");
            _resourceSubject.OnNext(addedResource);
            _resourceSubject.OnNext(removedResource);

            watcherResources.Should().BeEmpty();
        }

        [Fact]
        public void RaisesOnConnected()
        {
            _watcher.OnConnectedTriggered.Should().BeTrue();
        }

        [Fact]
        public void RaisesOnConnectionError()
        {
            _watcher.StartWatching();

            _resourceSubject.OnError(new Exception());

            _watcher.OnConnectionErrorTriggered.Should().BeTrue();
        }

        [Fact]
        public void ResubscribesOnError()
        {
            _watcher.StartWatching();

            _resourceSubject.OnError(new Exception());

            _resourceClientMock.Verify(mock => mock.Watch<Client>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ResubscribesOnCompletion()
        {
            _watcher.StartWatching();

            _resourceSubject.OnCompleted();

            _resourceClientMock.Verify(mock => mock.Watch<Client>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void DoesNotResubscribeAfterDisposal()
        {
            _watcher.StartWatching();

            _watcher.Dispose();
            _resourceSubject.OnCompleted();

            _resourceClientMock.Verify(mock => mock.Watch<Client>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void PassesLastResourceVersionOnReconnect()
        {
            _watcher.StartWatching();
            _resourceSubject.OnNext(CreateResourceEvent(ResourceEventType.Added, clientId: "4711", resourceVersion: "35"));

            _resourceSubject.OnError(new Exception());

            _resourceClientMock.Verify(mock => mock.Watch<Client>(It.IsAny<string>(), It.IsAny<string>(), "35"));
        }

        [Fact]
        public void DropsCacheWhenResourceIsGone()
        {
            _watcher.StartWatching();
            _resourceSubject.OnNext(CreateResourceEvent(ResourceEventType.Added, clientId: "4711", resourceVersion: "35"));

            _resourceSubject.OnError(new HttpRequestException<StatusV1>(HttpStatusCode.Gone, new StatusV1()));

            _watcher.Resources.Should().BeEmpty();
        }

        private static ResourceEventV1<CustomResource<Client>> CreateResourceEvent(ResourceEventType eventType, string clientId, string resourceVersion = "1")
            => new ResourceEventV1<CustomResource<Client>>
            {
                EventType = eventType,
                Resource = new CustomResource<Client>
                {
                    Metadata = new ObjectMetaV1
                    {
                        Namespace = "namespace",
                        Name = "identityclient",
                        ResourceVersion = resourceVersion
                    },
                    Spec = new Client {ClientId = clientId}
                }
            };

        private class TestResourceWatcher : CustomResourceWatcher<Client>
        {
            public TestResourceWatcher(ICustomResourceClient client)
                : base(new Logger<CustomResourceWatcher<Client>>(new LoggerFactory()), client, apiGroup: "stable.contrib.identityserver.io", crdPluralName: "identityclients")
            {
                OnConnected += (sender, args) => OnConnectedTriggered = true;
                OnConnectionError += (sender, args) => OnConnectionErrorTriggered = true;
            }

            public bool OnConnectedTriggered { get; private set; }
            public bool OnConnectionErrorTriggered { get; private set; }
        }
    }
}
