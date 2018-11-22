using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.JsonPatch;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClientResource : CustomResource<Client>, IPayloadPatchable<ClientResource>
    {
        public new static CustomResourceDefinition Definition { get; } = Crd.For(pluralName: "identityclients", kind: "IdentityClient");

        public ClientResource()
            : base(Definition)
        {}

        public ClientResource(string @namespace, string name, Client spec)
            : base(Definition, @namespace, name, spec)
        {}

        private static readonly CompareLogic SpecComparer = new CompareLogic();

        protected override bool SpecEquals(Client other)
            => SpecComparer.Compare(Spec, other).AreEqual;

        public void ToPayloadPatch(JsonPatchDocument<ClientResource> patch)
            => patch.Replace(x => x.Spec, Spec);
    }
}
