namespace Contrib.IdentityServer4.KubernetesStore
{
    public class KubernetesConfigurationStoreOptions
    {
        /// <summary>
        /// The connection string which points to the Kubernetes cluster.
        /// If not set, the service assumes that it runs inside the kubernetes cluster and autoconfigures itself.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
