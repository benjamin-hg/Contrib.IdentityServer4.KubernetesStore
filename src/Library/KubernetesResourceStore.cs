using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesResourceStore : InMemoryResourcesStore
    {
        public KubernetesResourceStore(IEnumerable<IdentityResource> identityResources, ICustomResourceWatcher<ApiResource> apiResourceWatcher)
            : base(identityResources, apiResourceWatcher.Resources)
        {
            apiResourceWatcher.StartWatching();
        }
    }
}
