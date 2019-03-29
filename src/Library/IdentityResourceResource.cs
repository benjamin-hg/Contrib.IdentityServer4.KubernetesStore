using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.JsonPatch;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class IdentityResourceResource : CustomResource<IdentityResource>, IPayloadPatchable<IdentityResourceResource>
    {
        public new static CustomResourceDefinition Definition { get; }
            = new CustomResourceDefinition("contrib.identityserver.io/v1", "identityresources", "IdentityResource");

        public IdentityResourceResource()
            : base(Definition)
        {}

        public IdentityResourceResource(string @namespace, string name, IdentityResource spec)
            : base(Definition, @namespace, name, spec)
        {}

        private static readonly CompareLogic SpecComparer = new CompareLogic();

        protected override bool SpecEquals(IdentityResource other)
            => SpecComparer.Compare(Spec, other).AreEqual;

        public void ToPayloadPatch(JsonPatchDocument<IdentityResourceResource> patch)
            => patch.Replace(x => x.Spec, Spec);
    }
}
