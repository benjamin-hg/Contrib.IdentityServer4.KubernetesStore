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
            : base(
                logger,
                identityResourceWatcher.Select(GetIdentityResource).Concat(defaultIdentityResources ?? Enumerable.Empty<IdentityResource>()),
                apiResourceWatcher.Select(GetApiResource))
        {}

        private static IdentityResource GetIdentityResource(IdentityResourceResource resource)
        {
            var identityResource = resource.Spec;
            if (string.IsNullOrEmpty(identityResource.Name))
                identityResource.Name = resource.Metadata.Namespace + "-" + resource.Metadata.Name;

            return identityResource;
        }

        private static ApiResource GetApiResource(ApiResourceResource resource)
        {
            var apiResource = resource.Spec;
            if (string.IsNullOrEmpty(apiResource.Name))
                apiResource.Name = resource.Metadata.Namespace + "-" + resource.Metadata.Name;

            return apiResource;
        }
    }
}
