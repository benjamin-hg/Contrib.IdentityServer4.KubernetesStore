using Contrib.KubeClient.CustomResources;

namespace Contrib.IdentityServer4.KubernetesStore
{
    internal static class Crd
    {
        public static CustomResourceDefinition For(string pluralName, string kind)
            => new CustomResourceDefinition("stable.contrib.identityserver.io/v1", pluralName, kind);
    }
}
