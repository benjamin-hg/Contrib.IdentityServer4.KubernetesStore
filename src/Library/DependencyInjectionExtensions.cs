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
            => services.AddCustomResourceClient<ClientResource>()
                       .AddCustomResourceClient<ApiResourceResource>()
                       .AddCustomResourceClient<IdentityResourceResource>();

        /// <summary>
        /// Adds <see cref="ICustomResourceWatcher{TResource}"/>s for <see cref="ClientResource"/>s and <see cref="ApiResourceResource"/>s.
        /// </summary>
        public static IServiceCollection AddIdentityResourceWatchers(this IServiceCollection services)
            => services.AddCustomResourceWatcher<ClientResource>()
                       .AddCustomResourceWatcher<ApiResourceResource>()
                       .AddCustomResourceWatcher<IdentityResourceResource>();

        /// <summary>
        /// Adds <see cref="IClientStore"/>, <see cref="IResourceStore"/> and <see cref="ICorsPolicyService"/> implementations backed by Kubernetes Custom Resources.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="defaultIdentityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IServiceCollection AddIdentityKubernetesStores(this IServiceCollection services, params IdentityResource[] defaultIdentityResources)
        {
            services.AddIdentityResourceWatchers()
                    .AddSingleton<IClientStore, KubernetesClientStore>()
                    .AddSingleton<IResourceStore, KubernetesResourceStore>()
                    .AddSingleton<ICorsPolicyService, KubernetesCorsPolicyService>()
                    .TryAddSingleton(defaultIdentityResources.AsEnumerable());
            return services;
        }

        /// <summary>
        /// Adds <see cref="IClientStore"/>, <see cref="IResourceStore"/> and <see cref="ICorsPolicyService"/> implementations backed by Kubernetes Custom Resources.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="defaultIdentityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder, params IdentityResource[] defaultIdentityResources)
        {
            builder.Services.AddIdentityKubernetesStores(defaultIdentityResources);
            return builder;
        }
    }
}
