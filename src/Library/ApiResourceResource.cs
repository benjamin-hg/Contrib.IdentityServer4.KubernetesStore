using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.JsonPatch;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ApiResourceResource : CustomResource<ApiResource>, IPayloadPatchable<ApiResourceResource>
    {
        public new static CustomResourceDefinition Definition { get; } = Crd.For(pluralName: "apiresources", kind: "ApiResource");

        public ApiResourceResource()
            : base(Definition)
        {}

        public ApiResourceResource(string @namespace, string name, ApiResource spec)
            : base(Definition, @namespace, name, spec)
        {}

        private static readonly CompareLogic SpecComparer = new CompareLogic();

        protected override bool SpecEquals(ApiResource other)
            => SpecComparer.Compare(Spec, other).AreEqual;

        public void ToPayloadPatch(JsonPatchDocument<ApiResourceResource> patch)
            => patch.Replace(x => x.Spec, Spec);
    }
}
