using System.Diagnostics.CodeAnalysis;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesClientStore : InMemoryClientStore
    {
        public KubernetesClientStore(ICustomResourceWatcher<Client> clientWatcher)
            : base(clientWatcher.Resources)
        {
            clientWatcher.StartWatching();
        }
    }
}
