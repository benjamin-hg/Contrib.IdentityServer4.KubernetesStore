using System.Collections.Generic;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class DependencyInjectionFacts
    {
        [Fact]
        public void ServicesAreRegistered()
        {
            var provider = new ServiceCollection()
                          .AddLogging()
                          .Configure<KubernetesConfigurationStoreOptions>(opt => opt.ConnectionString = "http://example.com/")
                          .AddIdentityKubernetesConfigurationStore()
                          .BuildServiceProvider();

            provider.GetRequiredService<ICustomResourceWatcher<ClientResource>>();
            provider.GetRequiredService<ICustomResourceWatcher<ApiResourceResource>>();
            provider.GetRequiredService<IClientStore>();
            provider.GetRequiredService<IResourceStore>();
            provider.GetRequiredService<ICorsPolicyService>();
            provider.GetRequiredService<IEnumerable<IdentityResource>>();
        }
    }
}
