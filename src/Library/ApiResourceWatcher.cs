using System.Diagnostics.CodeAnalysis;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class ApiResourceWatcher : CustomResourceWatcher<ApiResource>
    {
        public ApiResourceWatcher(ILogger<CustomResourceWatcher<ApiResource>> logger, ICustomResourceClient client)
            : base(logger, client, apiGroup: "stable.contrib.identityserver.io", crdPluralName: "identityapiresources", @namespace: string.Empty)
        {}
    }
}
