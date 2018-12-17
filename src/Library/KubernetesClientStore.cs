using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesClientStore : InMemoryClientStore
    {
        public KubernetesClientStore(ICustomResourceWatcher<ClientResource> clientWatcher)
            : base(clientWatcher.Select(resource => resource.Spec))
        {}
    }
}
