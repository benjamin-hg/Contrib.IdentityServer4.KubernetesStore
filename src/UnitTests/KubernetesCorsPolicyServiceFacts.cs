using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contrib.KubeClient.CustomResources;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class KubernetesCorsPolicyServiceFacts
    {
        private readonly Mock<ILogger<KubernetesCorsPolicyService>> _loggerMock;

        private readonly List<ClientResource> _clientResources = new List<ClientResource>
        {
            new ClientResource("ns", "dummy", new Client
            {
                ClientName = "NormalClient1",
                AllowedCorsOrigins = {"http://example1.com", "https://another.example1.com"}
            }),
            new ClientResource("ns", "dummy", new Client
            {
                ClientName = "CorruptClient",
                AllowedCorsOrigins = {"http://example0.com", "not // .a.valid }} URI", "https://another.example0.com"}
            }),
            new ClientResource("ns", "dummy", new Client()
            {
                ClientName = "NormalClient2",
                AllowedCorsOrigins = {"http://example2.com", "https://another.example2.com"}
            })
        };

        private readonly Mock<ICustomResourceWatcher<ClientResource>> _clientResourceWatcherMock;

        public KubernetesCorsPolicyServiceFacts()
        {
            _loggerMock = new Mock<ILogger<KubernetesCorsPolicyService>>();
            _clientResourceWatcherMock = new Mock<ICustomResourceWatcher<ClientResource>>();
            _clientResourceWatcherMock.Setup(x => x.RawResources).Returns(_clientResources);
        }

        private KubernetesCorsPolicyService SubjectUnderTest => new KubernetesCorsPolicyService(_loggerMock.Object, _clientResourceWatcherMock.Object);

        [Fact]
        public async Task ClientsWithInvalidAllowedCorsOriginsAreLoggedAsWarning()
        {
            string logMessage = null;
            _loggerMock.SetupLog(LogLevel.Warning).Callback((level, id, state, ex, formatter) => logMessage = state.ToString());

            await SubjectUnderTest.IsOriginAllowedAsync("http://localhost");

            _loggerMock.VerifyLog(LogLevel.Warning, Times.Once());
            logMessage.Should().Contain("CorruptClient");
            logMessage.Should().NotContain("NormalClient");
        }

        [Fact]
        public async Task InvalidEntriesDontAffectChecksAgainstValidEntries()
        {
            SubjectUnderTest.Awaiting(x => x.IsOriginAllowedAsync("http://example1.com")).Should().NotThrow<UriFormatException>();
            (await SubjectUnderTest.IsOriginAllowedAsync("https://another.example2.com")).Should().Be(true);
            (await SubjectUnderTest.IsOriginAllowedAsync("http://example0.com")).Should().Be(true);
        }
    }
}
