using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class DuplicateScopeFilteringInMemoryResourcesStore : InMemoryResourcesStore
    {
        public DuplicateScopeFilteringInMemoryResourcesStore(
            ILogger logger,
            IEnumerable<IdentityResource> identityResources = null,
            IEnumerable<ApiResource> apiResources = null)
            : base(EnsureUniqueIndentityScopeNames(identityResources, logger, apiResources), EnsureUniqueApiResourceScopeNames(apiResources, logger, identityResources)) { }

        internal static IEnumerable<ApiResource> EnsureUniqueApiResourceScopeNames(IEnumerable<ApiResource> apiResources, ILogger logger, IEnumerable<IdentityResource> identityResources)
        {
            var allApiScopeNamesSoFar = new List<string>();
            var identityScopeNames = identityResources.Select(s => s.Name).ToList();
            foreach (var apiResource in apiResources)
            {
                var resultApiResource = apiResource;
                foreach (var apiResourceScope in apiResource.Scopes)
                {
                    if (allApiScopeNamesSoFar.Contains(apiResourceScope.Name))
                    {
                        logger.LogError($"Duplicate Identity scope found: '{apiResource.Name}'. This is an invalid configuration. Use different names for Identity scopes.");
                        resultApiResource = CloneWithScopesExcept(resultApiResource, apiResourceScope);
                    }
                    else if (identityScopeNames.Contains(apiResourceScope.Name))
                    {
                        resultApiResource = CloneWithScopesExcept(resultApiResource, apiResourceScope);
                        logger.LogError($"Duplicate Identity scope name found: '{apiResource.Name}', it is used by another API scope. This is an invalid configuration. Use different names for Identity and API scopes.");
                    }
                    else
                    {
                        allApiScopeNamesSoFar.Add(apiResourceScope.Name);
                    }
                }
                yield return resultApiResource;
            }
        }

        private static ApiResource CloneWithScopesExcept(ApiResource original, Scope apiScopeToExclude)
        {
            var result = Clone(original);
            result.Scopes = result.Scopes.Where(s => s != apiScopeToExclude).ToList();
            return result;
        }

        private static ApiResource Clone(ApiResource original)
        {
            return new ApiResource
            {
                Name = original.Name,
                Scopes = original.Scopes,
                UserClaims = original.UserClaims,
                DisplayName = original.DisplayName,
                Description = original.Description,
                ApiSecrets = original.ApiSecrets,
                Enabled = original.Enabled,
                Properties = original.Properties
            };
        }

        internal static IEnumerable<IdentityResource> EnsureUniqueIndentityScopeNames(IEnumerable<IdentityResource> identityResources, ILogger logger, IEnumerable<ApiResource> apiResources)
        {
            var allIdentityScopeNamesSoFar = new List<string>();
            var apiScopeNames = apiResources.SelectMany(r => r.Scopes.Select(s => s.Name)).ToList();
            foreach (var identityResource in identityResources)
            {
                if (allIdentityScopeNamesSoFar.Contains(identityResource.Name))
                {
                    logger.LogError($"Duplicate Identity scope found: '{identityResource.Name}'. This is an invalid configuration. Use different names for Identity scopes.");
                }
                else if (apiScopeNames.Contains(identityResource.Name))
                {
                    logger.LogError($"Duplicate Identity scope name found: '{identityResource.Name}', it is used by another API scope. This is an invalid configuration. Use different names for Identity and API scopes.");
                }
                else
                {
                    allIdentityScopeNamesSoFar.Add(identityResource.Name);
                    yield return identityResource;
                }
            }
        }

    }
}
