using System.Diagnostics.CodeAnalysis;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class IdentityResourceWatcher : CustomResourceWatcher<IdentityResource>
    {
        public IdentityResourceWatcher(ILogger<CustomResourceWatcher<IdentityResource>> logger, ICustomResourceClient client)
            : base(logger, client, apiGroup: "stable.contrib.identityserver.io", crdPluralName: "identityresources", @namespace: string.Empty)
        {}
    }
}
