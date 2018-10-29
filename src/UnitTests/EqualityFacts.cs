using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class EqualityFacts
    {
        [Fact]
        public void DetectsEqualClients()
        {
            var client1 = new ClientResource("ns", "client", new Client
            {
                AllowOfflineAccess = true,
                AllowedCorsOrigins =
                {
                    "http://example.com"
                }
            });
            var client2 = new ClientResource("ns", "client", new Client
            {
                AllowOfflineAccess = true,
                AllowedCorsOrigins =
                {
                    "http://example.com"
                }
            });

            client1.Equals(client2).Should().BeTrue();
        }

        [Fact]
        public void DetectsUnequalClients()
        {
            var client1 = new ClientResource("ns", "client", new Client
            {
                AllowOfflineAccess = true,
                AllowedCorsOrigins =
                {
                    "http://example.com/1"
                }
            });
            var client2 = new ClientResource("ns", "client", new Client
            {
                AllowOfflineAccess = true,
                AllowedCorsOrigins =
                {
                    "http://example.com/1",
                    "http://example.com/2",
                }
            });

            client1.Equals(client2).Should().BeFalse();
        }

        [Fact]
        public void DetectsEqualApiResources()
        {
            var apiResource1 = new ApiResourceResource("ns", "resource", new ApiResource
            {
                ApiSecrets = {new Secret("secret")}
            });
            var apiResource2 = new ApiResourceResource("ns", "resource", new ApiResource
            {
                ApiSecrets = {new Secret("secret")}
            });

            apiResource1.Equals(apiResource2).Should().BeTrue();
        }

        [Fact]
        public void DetectsUnequalApiResources()
        {
            var apiResource1 = new ApiResourceResource("ns", "resource", new ApiResource
            {
                ApiSecrets = {new Secret("secret1")}
            });
            var apiResource2 = new ApiResourceResource("ns", "resource", new ApiResource
            {
                ApiSecrets = {new Secret("secret2")}
            });

            apiResource1.Equals(apiResource2).Should().BeFalse();
        }
    }
}
