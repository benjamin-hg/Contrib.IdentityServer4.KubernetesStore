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
            : base(logger, clientWatcher.Select(resource => GetClient(resource, logger)))
        { }

        private static Client GetClient(ClientResource resource, ILogger logger)
        {
            var client = resource.Spec;
            if (string.IsNullOrEmpty(client.ClientId))
                client.ClientId = resource.Metadata.Namespace + "-" + resource.Metadata.Name;

            if (client.AllowedCorsOrigins.All(IsWellFormedUriString))
                return client;

            logger.LogWarning($"Identity Client with name {client.ClientName} has invalid AllowedCorsOrigins");

            return new Client
            {
                ClientName = client.ClientName,
                AllowedCorsOrigins = client.AllowedCorsOrigins.Where(IsWellFormedUriString).ToList()
            };
        }

        private static bool IsWellFormedUriString(string origin) => Uri.IsWellFormedUriString(origin, UriKind.RelativeOrAbsolute);
    }
}
