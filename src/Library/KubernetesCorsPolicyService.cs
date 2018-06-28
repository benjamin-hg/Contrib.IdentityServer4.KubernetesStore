using System.Diagnostics.CodeAnalysis;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesCorsPolicyService : InMemoryCorsPolicyService
    {
        public KubernetesCorsPolicyService(ILogger<KubernetesCorsPolicyService> logger, ICustomResourceWatcher<Client> clientWatcher)
            : base(logger, clientWatcher.Resources)
        {
            clientWatcher.StartWatching();
        }
    }
}
