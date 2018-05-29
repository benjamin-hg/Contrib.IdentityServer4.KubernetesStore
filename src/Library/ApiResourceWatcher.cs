using System.Diagnostics.CodeAnalysis;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class ApiResourceWatcher : CustomResourceWatcher<ApiResource>
    {
        public ApiResourceWatcher(ILogger<CustomResourceWatcher<ApiResource>> logger, ICustomResourceClient client)
            : base(logger, client, crdPluralName: "identityapiresources")
        {}
    }
}
