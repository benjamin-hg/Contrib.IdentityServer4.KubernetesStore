using System.Collections.Generic;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using KubeClient;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class DependencyInjectionFacts
    {
        [Fact]
        public void ServicesAreRegistered()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                    .AddKubeClient(new KubeClientOptions("http://example.com/"));
            services.AddIdentityResourceWatchers()
                    .AddIdentityKubernetesStores();

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<ICustomResourceClient<ClientResource>>();
            provider.GetRequiredService<ICustomResourceClient<ApiResourceResource>>();
            provider.GetRequiredService<ICustomResourceWatcher<ClientResource>>();
            provider.GetRequiredService<ICustomResourceWatcher<ApiResourceResource>>();
            provider.GetRequiredService<IClientStore>();
            provider.GetRequiredService<IResourceStore>();
            provider.GetRequiredService<ICorsPolicyService>();
            provider.GetRequiredService<IEnumerable<IdentityResource>>();
        }
    }
}
