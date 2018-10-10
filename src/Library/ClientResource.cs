using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClientResource : CustomResource<Client>
    {
        public static CustomResourceDefinition<ClientResource> Definition { get; } = new CustomResourceDefinition<ClientResource>(apiVersion: "stable.contrib.identityserver.io/v1", pluralName: "identityclients");

        public ClientResource()
        {}

        public ClientResource(string @namespace, string name)
            : base(@namespace, name)
        {}

        public ClientResource(string @namespace, string name, Client spec)
            : base(@namespace, name, spec)
        {}

        public ClientResource(Client spec)
            : base(spec)
        {}
    }
}
