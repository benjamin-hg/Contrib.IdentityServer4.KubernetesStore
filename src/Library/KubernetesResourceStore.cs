using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesResourceStore : DuplicateScopeFilteringInMemoryResourcesStore
    {
        public KubernetesResourceStore(ILogger<KubernetesResourceStore> logger, ICustomResourceWatcher<IdentityResourceResource> identityResourceWatcher, ICustomResourceWatcher<ApiResourceResource> apiResourceWatcher, IEnumerable<IdentityResource> defaultIdentityResources = null)
            : base(logger, identityResourceWatcher.Select(resource => resource.Spec).Concat(defaultIdentityResources ?? Enumerable.Empty<IdentityResource>()), apiResourceWatcher.Select(resource => resource.Spec))
        { }

    }
}
