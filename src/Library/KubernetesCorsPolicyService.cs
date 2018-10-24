using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesCorsPolicyService : InMemoryCorsPolicyService
    {
        public KubernetesCorsPolicyService(ILogger<KubernetesCorsPolicyService> logger, ICustomResourceWatcher<ClientResource> clientWatcher)
            : base(logger, clientWatcher.RawResources.Select(resource => resource.Spec).Where(client => HasWellFormedAllowedCorsOrigins(client, logger)))
        { }

        private static bool HasWellFormedAllowedCorsOrigins(Client client, ILogger logger)
        {
            var hasWellFormedAllowedCorsOrigins = client.AllowedCorsOrigins.All(origin => Uri.IsWellFormedUriString(origin, UriKind.RelativeOrAbsolute));
            if (hasWellFormedAllowedCorsOrigins)
            {
                return true;
            }
            else
            {
                logger.LogWarning($"Identity Client with name {client.ClientName} has invalid AllowedCorsOrigins");
                return false;
            }
        }
    }
}
