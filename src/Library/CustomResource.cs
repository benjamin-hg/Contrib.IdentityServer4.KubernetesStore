using System.Diagnostics.CodeAnalysis;
using KubeClient.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class CustomResource<TSpec> : KubeResourceV1
    {
        public TSpec Spec { get; set; }

        public StatusV1 Status { get; set; }

        public string GlobalName => $"{Metadata.Namespace ?? "[cluster]"}.{Metadata.Name}";
    }
}
