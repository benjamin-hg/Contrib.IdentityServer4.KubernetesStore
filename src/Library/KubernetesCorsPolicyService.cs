using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesCorsPolicyService : InMemoryCorsPolicyService
    {
        public KubernetesCorsPolicyService(ILogger<KubernetesCorsPolicyService> logger, ICustomResourceWatcher<ClientResource> clientWatcher)
            : base(logger, clientWatcher.RawResources.Select(resource => resource.Spec))
        {}
    }
}
