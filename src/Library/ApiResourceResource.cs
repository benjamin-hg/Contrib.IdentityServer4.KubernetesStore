using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ApiResourceResource : CustomResource<ApiResource>
    {
        public static CustomResourceDefinition<ApiResourceResource> Definition { get; } = new CustomResourceDefinition<ApiResourceResource>(apiVersion: "stable.contrib.identityserver.io/v1", pluralName: "identityapiresources");

        public ApiResourceResource()
        {}

        public ApiResourceResource(string @namespace, string name)
            : base(@namespace, name)
        {}

        public ApiResourceResource(string @namespace, string name, ApiResource spec)
            : base(@namespace, name, spec)
        {}

        public ApiResourceResource(ApiResource spec) : base(spec)
        {}
    }
}
