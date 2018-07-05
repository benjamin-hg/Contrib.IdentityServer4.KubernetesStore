using System;
using System.Diagnostics.CodeAnalysis;
using HTTPlease;
using KubeClient;
using KubeClient.Models;
using KubeClient.ResourceClients;
using Microsoft.Extensions.Options;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class CustomResourceClient : KubeResourceClient, ICustomResourceClient
    {
        private readonly string _kubeNamespace;

        public CustomResourceClient(IOptions<KubernetesConfigurationStoreOptions> options, IKubeApiClient client)
            : base(client)
        {
            _kubeNamespace = options.Value.Namespace ?? string.Empty;
        }

        public IObservable<IResourceEventV1<CustomResource<TSpec>>> Watch<TSpec>(string apiGroup, string crdPluralName, string lastSeenResourceVersion)
        {
            string resourceVersion = string.Empty;

            if (!string.IsNullOrWhiteSpace(lastSeenResourceVersion))
                resourceVersion = $"&resourceVersion={lastSeenResourceVersion}";

            return ObserveEvents<CustomResource<TSpec>>(KubeRequest.Create($"/apis/{apiGroup}/v1/namespaces/{_kubeNamespace}/{crdPluralName}?watch=true{resourceVersion}"));
        }
    }
}
