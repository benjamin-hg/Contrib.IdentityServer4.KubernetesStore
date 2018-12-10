using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class DuplicateScopeFilteringInMemoryResourcesStoreFacts
    {
        private readonly Mock<ILogger<DuplicateScopeFilteringInMemoryResourcesStore>> _loggerMock;

        public DuplicateScopeFilteringInMemoryResourcesStoreFacts()
        {
            _loggerMock = new Mock<ILogger<DuplicateScopeFilteringInMemoryResourcesStore>>();
        }

        [Fact]
        public void EnsureUniqueIndentityScopeNames_LogsNoErrorForNoIdentityResource()
        {
            var identityResources = new List<IdentityResource>();

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, new List<ApiResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueIndentityScopeNames_LogsNoErrorForSingleIdentityResource()
        {
            var identityResource = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResources = new List<IdentityResource>() { identityResource };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, new List<ApiResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueIndentityScopeNames_LogsNoErrorForIdentityResourcesWithDistinctNames()
        {
            var identityResource1 = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResource2 = new IdentityResource("IdentityResource2", new[] { "" });
            var identityResources = new List<IdentityResource>() { identityResource1, identityResource2 };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, new List<ApiResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueIndentityScopeNames_LogsErrorForIdentityResourcesWithSameName()
        {
            var identityResource1 = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResource2 = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResources = new List<IdentityResource>() { identityResource1, identityResource2 };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, new List<ApiResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public void EnsureUniqueIndentityScopeNames_ReturnsOnlyIdentityResourcesWithDistinctNames()
        {
            var identityResource1 = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResource2 = new IdentityResource("IdentityResource1", new[] { "" });
            var identityResource3 = new IdentityResource("IdentityResource3", new[] { "" });
            var identityResources = new List<IdentityResource>() { identityResource1, identityResource2, identityResource3 };

            var result = DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, new List<ApiResource>()).ToList();
            result.Select(r => r.Name).Should().OnlyHaveUniqueItems();
        }


        //############

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsNoErrorForNoApiResource()
        {
            var apiResources = new List<ApiResource>();

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsNoErrorForSingleApiResourceWithSingleScope()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}}
            };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsNoErrorForApiResourcesWithDistinctScopeNames()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource("scope2"){Scopes = {new Scope(){Name = "scope3"}}},
            };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Never());
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsErrorsForApiResourcesWithDuplicateScopeNames()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource("scope1"){Scopes = {new Scope(){Name = "scope3"}, new Scope(){Name = "scope3"}}},
            };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Exactly(2));
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsErrorForApiResourcesWithSameScopeName()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
            };
            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_ReturnsOnlyApiResourcesWithDistinctNames()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource("ApiResource1"){Scopes = { new Scope() { Name = "scope1" }, new Scope() { Name = "scope3" }, new Scope(){Name = "scope3"}}},
            };

            var result = DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource>()).ToList();
            result.Should().BeEquivalentTo(new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource("ApiResource1"){Scopes = {new Scope(){Name = "scope3"}}},
            });
        }

        [Fact]
        public void EnsureUniqueApiResourceScopeNames_LogsErrorWhenApiResourceHasSameScopeNameAsIdentityResource()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource(){Scopes = {new Scope(){Name = "scope1"}}},
                new ApiResource(){Scopes = {new Scope(){Name = "scope2"}}},
            };
            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueApiResourceScopeNames(apiResources, _loggerMock.Object, new List<IdentityResource> { new IdentityResource("scope1", new []{""}) }).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }


        [Fact]
        public void EnsureUniqueIndentityScopeNames_LogsErrorWhenApiResourceHasSameScopeNameAsIdentityResource()
        {
            var identityResource1 = new IdentityResource("scope1", new[] { "" });
            var identityResource2 = new IdentityResource("scope2", new[] { "" });
            var identityResources = new List<IdentityResource> { identityResource1, identityResource2 };

            var apiResources = new List<ApiResource> { new ApiResource { Scopes = { new Scope { Name = "scope1" } } } };

            DuplicateScopeFilteringInMemoryResourcesStore
                .EnsureUniqueIndentityScopeNames(identityResources, _loggerMock.Object, apiResources).ToList();
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

    }
}
