using System;
using System.Reactive.Subjects;
using FluentAssertions;
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
            _resourceClientMock.Setup(mock => mock.Watch<Client>("identityclients")).Returns(_resourceSubject);
            _watcher = new TestResourceWatcher(_resourceClientMock.Object);
        }

        [Fact]
        public void AddedResourceGetsPopulatedToResources()
        {
            var expectedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");

            _resourceSubject.OnNext(expectedResource);

            _watcher.Resources.Should().Contain(expectedResource.Resource.Spec);
        }

        [Fact]
        public void ModifiedResourceGetsPopulatedToResources()
        {
            var addedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");
            var modifiedResource = CreateResourceEvent(ResourceEventType.Modified, "expectedClientId");
            modifiedResource.Resource.Spec.ClientName = "clientname";
            _resourceSubject.OnNext(addedResource);

            _resourceSubject.OnNext(modifiedResource);

            _watcher.Resources.Should().Contain(modifiedResource.Resource.Spec);
        }

        [Fact]
        public void DeletedResourceGetsRemovedFromResources()
        {
            var addedResource = CreateResourceEvent(ResourceEventType.Added, "expectedClientId");
            var removedResource = CreateResourceEvent(ResourceEventType.Deleted, "expectedClientId");
            _resourceSubject.OnNext(addedResource);

            _resourceSubject.OnNext(removedResource);

            _watcher.Resources.Should().BeEmpty();
        }

        [Fact]
        public void ResubscribesOnError()
        {
            var errorSubject = new Subject<IResourceEventV1<CustomResource<Client>>>();
            var resourceClientMock = new Mock<ICustomResourceClient>();
            resourceClientMock.SetupSequence(mock => mock.Watch<Client>(It.IsAny<string>()))
                              .Returns(errorSubject)
                              .Returns(new Subject<IResourceEventV1<CustomResource<Client>>>());
            var _ = new TestResourceWatcher(resourceClientMock.Object);

            errorSubject.OnError(new Exception());

            resourceClientMock.Verify(mock => mock.Watch<Client>(It.IsAny<string>()), Times.Exactly(2));
        }

        private static ResourceEventV1<CustomResource<Client>> CreateResourceEvent(ResourceEventType eventType, string clientId)
            => new ResourceEventV1<CustomResource<Client>>
            {
                EventType = eventType,
                Resource = new CustomResource<Client>
                {
                    Metadata = new ObjectMetaV1
                    {
                        Namespace = "namespace",
                        Name = "identityclient"
                    },
                    Spec = new Client {ClientId = clientId}
                }
            };

        private class TestResourceWatcher : CustomResourceWatcher<Client>
        {
            public TestResourceWatcher(ICustomResourceClient client)
                : base(new Logger<CustomResourceWatcher<Client>>(new LoggerFactory()), client, "identityclients")
            {}
        }
    }
}
