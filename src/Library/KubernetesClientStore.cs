using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesClientStore : InMemoryClientStore
    {
        public KubernetesClientStore(ICustomResourceWatcher<ClientResource> clientWatcher)
            : base(clientWatcher.Select(GetClient))
        {}

        private static Client GetClient(ClientResource resource)
        {
            var client = resource.Spec;
            if (string.IsNullOrEmpty(client.ClientId))
                client.ClientId = resource.Metadata.Namespace + "-" + resource.Metadata.Name;

            return client;
        }
    }
}
