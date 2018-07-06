using System;
using KubeClient.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public interface ICustomResourceClient
    {
        IObservable<IResourceEventV1<CustomResource<TSpec>>> Watch<TSpec>(string apiGroup, string crdPluralName, string @namespace = "", string lastSeenResourceVersion = "0");
    }
}
