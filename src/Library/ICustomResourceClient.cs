using System;
using KubeClient.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public interface ICustomResourceClient
    {
        IObservable<IResourceEventV1<CustomResource<TSpec>>> Watch<TSpec>(string crdPluralName);
        IObservable<IResourceEventV1<CustomResource<TSpec>>> Watch<TSpec>(string crdPluralName, string lastSeenResourceVersion);
    }
}
