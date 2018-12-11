using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class DuplicateScopeFilteringInMemoryResourcesStore : InMemoryResourcesStore
    {
        public DuplicateScopeFilteringInMemoryResourcesStore(
            ILogger logger,
            IEnumerable<IdentityResource> identityResources = null,
            IEnumerable<ApiResource> apiResources = null)
            : base(EnsureUniqueIndentityScopeNames(identityResources, logger, apiResources), EnsureUniqueApiResourceScopeNames(apiResources, logger, identityResources))
        {}

        internal static IEnumerable<ApiResource> EnsureUniqueApiResourceScopeNames(IEnumerable<ApiResource> apiResources, ILogger logger, IEnumerable<IdentityResource> identityResources)
        {
            var allApiScopeNamesSoFar = new HashSet<string>();
            var identityScopeNames = new HashSet<string>(identityResources.Select(s => s.Name));
            foreach (var apiResource in apiResources)
            {
                var resultApiResource = apiResource;
                foreach (var apiResourceScope in apiResource.Scopes)
                    if (allApiScopeNamesSoFar.Add(apiResourceScope.Name))
                    {
                        if (identityScopeNames.Contains(apiResourceScope.Name))
                        {
                            resultApiResource = CloneWithScopesExcept(resultApiResource, apiResourceScope);
                            logger.LogError($"Duplicate Identity scope name found: '{apiResource.Name}', it is used by another API scope. This is an invalid configuration. Use different names for Identity and API scopes.");
                        }
                    }
                    else
                    {
                        logger.LogError($"Duplicate Identity scope found: '{apiResource.Name}'. This is an invalid configuration. Use different names for Identity scopes.");
                        resultApiResource = CloneWithScopesExcept(resultApiResource, apiResourceScope);
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
            var allIdentityScopeNamesSoFar = new HashSet<string>();
            var apiScopeNames = new HashSet<string>(apiResources.SelectMany(r => r.Scopes.Select(s => s.Name)));
            foreach (var identityResource in identityResources)
                if (allIdentityScopeNamesSoFar.Add(identityResource.Name))
                    if (apiScopeNames.Contains(identityResource.Name))
                        logger.LogError($"Duplicate Identity scope name found: '{identityResource.Name}', it is used by another API scope. This is an invalid configuration. Use different names for Identity and API scopes.");
                    else
                        yield return identityResource;
                else
                    logger.LogError($"Duplicate Identity scope found: '{identityResource.Name}'. This is an invalid configuration. Use different names for Identity scopes.");
        }
    }
}
