using System.Diagnostics.CodeAnalysis;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesResourceStore : InMemoryResourcesStore
    {
        public KubernetesResourceStore(ICustomResourceWatcher<IdentityResource> identityResourceWatcher, ICustomResourceWatcher<ApiResource> apiResourceWatcher)
            : base(identityResourceWatcher.Resources, apiResourceWatcher.Resources)
        {
            identityResourceWatcher.StartWatching();
            apiResourceWatcher.StartWatching();
        }
    }
}
