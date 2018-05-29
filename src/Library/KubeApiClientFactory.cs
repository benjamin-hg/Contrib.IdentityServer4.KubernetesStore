using KubeClient;
using Microsoft.Extensions.Options;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class KubeApiClientFactory
    {
        private readonly string _connectionString;

        public KubeApiClientFactory(IOptions<KubernetesConfigurationStoreOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public IKubeApiClient Build()
            => string.IsNullOrWhiteSpace(_connectionString)
                   ? KubeApiClient.CreateFromPodServiceAccount()
                   : KubeApiClient.Create(_connectionString);
    }
}
