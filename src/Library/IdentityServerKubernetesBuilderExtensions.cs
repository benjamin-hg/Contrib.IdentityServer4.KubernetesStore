using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contrib.IdentityServer4.KubernetesStore
{
    /// <summary>
    /// Extension methods to add Kubernetes support to IdentityServer.
    /// </summary>
    public static class IdentityServerKubernetesBuilderExtensions
    {
        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder)
            => builder.AddKubernetesConfigurationStore(new ConfigurationBuilder().Build());

        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configuration">The configuration section to be bound to <see cref="KubernetesConfigurationStoreOptions"/>.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.Services
                   .Configure<KubernetesConfigurationStoreOptions>(configuration)
                   .AddKubernetesClient()
                   .AddKubernetesResourceWatchers()
                   .AddKubernetesStores();

            return builder;
        }

        private static IServiceCollection AddKubernetesResourceWatchers(this IServiceCollection services)
            => services.AddSingleton<ICustomResourceWatcher<Client>, ClientWatcher>()
                       .AddSingleton<ICustomResourceWatcher<IdentityResource>, IdentityResourceWatcher>()
                       .AddSingleton<ICustomResourceWatcher<ApiResource>, ApiResourceWatcher>();

        private static IServiceCollection AddKubernetesStores(this IServiceCollection services)
            => services.AddSingleton<IClientStore, KubernetesClientStore>()
                       .AddSingleton<IResourceStore, KubernetesResourceStore>()
                       .AddSingleton<ICorsPolicyService, KubernetesCorsPolicyService>();
    }
}
