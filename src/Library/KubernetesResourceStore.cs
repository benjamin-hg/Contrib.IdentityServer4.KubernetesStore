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
        public KubernetesResourceStore(ICustomResourceWatcher<IdentityResourceResource> identityResourceWatcher, ICustomResourceWatcher<ApiResourceResource> apiResourceWatcher, IEnumerable<IdentityResource> defaultIdentityResources = null)
            : base(identityResourceWatcher.RawResources.Select(resource => resource.Spec).Concat(defaultIdentityResources), apiResourceWatcher.RawResources.Select(resource => resource.Spec))
        {}
    }
}
