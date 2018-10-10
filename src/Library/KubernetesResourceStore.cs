using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesResourceStore : InMemoryResourcesStore
    {
        public KubernetesResourceStore(IEnumerable<IdentityResource> identityResources, ICustomResourceWatcher<ApiResourceResource> apiResourceWatcher)
            : base(identityResources, apiResourceWatcher.RawResources.Select(x => x.Spec))
        {}
    }
}
