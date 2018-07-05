using System.Diagnostics.CodeAnalysis;
using KubeClient.Models;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class CustomResource<TSpec> : KubeResourceV1
    {
        public TSpec Spec { get; set; }

        public StatusV1 Status { get; set; }

        public string GlobalName
        {
            get
            {
                return string.IsNullOrWhiteSpace(Metadata.Namespace)
                           ? $"[cluster].{Metadata.Name}"
                           : $"{Metadata.Namespace}.{Metadata.Name}";
            }
        }
    }
}
