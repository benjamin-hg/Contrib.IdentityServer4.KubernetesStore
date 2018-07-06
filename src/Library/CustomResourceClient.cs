using System;
using System.Diagnostics.CodeAnalysis;
using HTTPlease;
using KubeClient;
using KubeClient.Models;
using KubeClient.ResourceClients;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class CustomResourceClient : KubeResourceClient, ICustomResourceClient
    {
        public CustomResourceClient(IKubeApiClient client)
            : base(client)
        {}

        public IObservable<IResourceEventV1<CustomResource<TSpec>>> Watch<TSpec>(string apiGroup, string crdPluralName, string @namespace = "", string lastSeenResourceVersion = "0")
        {
            var httpRequest = KubeRequest.Create($"/apis/{apiGroup}/v1/");

            if (!string.IsNullOrWhiteSpace(@namespace))
                httpRequest = httpRequest.WithRelativeUri($"namespaces/{@namespace}/");

            httpRequest = httpRequest
                         .WithRelativeUri($"{crdPluralName}")
                         .WithQueryParameter("watch", true)
                         .WithQueryParameter("resourceVersion", lastSeenResourceVersion);

            return ObserveEvents<CustomResource<TSpec>>(httpRequest);
        }
    }
}
