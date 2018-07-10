using System.Diagnostics.CodeAnalysis;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class ClientWatcher : CustomResourceWatcher<Client>
    {
        public ClientWatcher(ILogger<CustomResourceWatcher<Client>> logger, ICustomResourceClient client)
            : base(logger, client, apiGroup: "stable.contrib.identityserver.io", crdPluralName: "identityclients", @namespace: string.Empty)
        {}
    }
}
