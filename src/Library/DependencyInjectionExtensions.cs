using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Contrib.IdentityServer4.KubernetesStore
{
    /// <summary>
    /// Extension methods to add Kubernetes support to IdentityServer.
    /// </summary>
    [PublicAPI]
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds <see cref="ICustomResourceClient{TResource}"/>s for <see cref="ClientResource"/>s and <see cref="ApiResourceResource"/>s.
        /// </summary>
        public static IServiceCollection AddIdentityResourceClients(this IServiceCollection services)
            => services.AddCustomResourceClient(ClientResource.Definition)
                       .AddCustomResourceClient(ApiResourceResource.Definition);

        /// <summary>
        /// Adds <see cref="ICustomResourceWatcher{TResource}"/>s for <see cref="ClientResource"/>s and <see cref="ApiResourceResource"/>s.
        /// Remember to call <see cref="Contrib.KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        public static IServiceCollection AddIdentityResourceWatchers(this IServiceCollection services)
            => services.AddCustomResourceWatcher(ClientResource.Definition)
                       .AddCustomResourceWatcher(ApiResourceResource.Definition);

        /// <summary>
        /// Adds <see cref="IClientStore"/>, <see cref="IResourceStore"/> and <see cref="ICorsPolicyService"/> implementations backed by Kubernetes Custom Resources.
        /// Remember to call <see cref="Contrib.KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IServiceCollection AddIdentityKubernetesStores(this IServiceCollection services, params IdentityResource[] identityResources)
        {
            services.AddIdentityResourceWatchers()
                    .AddSingleton<IClientStore, KubernetesClientStore>()
                    .AddSingleton<IResourceStore, KubernetesResourceStore>()
                    .AddSingleton<ICorsPolicyService, KubernetesCorsPolicyService>()
                    .TryAddSingleton(identityResources.AsEnumerable());
            return services;
        }

        /// <summary>
        /// Adds <see cref="IClientStore"/>, <see cref="IResourceStore"/> and <see cref="ICorsPolicyService"/> implementations backed by Kubernetes Custom Resources.
        /// Remember to call <see cref="Contrib.KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder, params IdentityResource[] identityResources)
        {
            builder.Services.AddIdentityKubernetesStores(identityResources);
            return builder;
        }
    }
}
