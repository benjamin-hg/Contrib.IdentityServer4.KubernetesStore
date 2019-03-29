using System.Security.Claims;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClaimConverterFacts : ConverterFacts<Claim>
    {
        public ClaimConverterFacts() : base(new ClaimConverter())
        {}

        [Fact]
        public void WritesAllToJson() 
            => ExpectJson(
                new Claim(type: "mytype", value: "myvalue", valueType: "myvaluetype", issuer: "myissuer"),
                "{\"type\":\"mytype\",\"value\":\"myvalue\",\"valueType\":\"myvaluetype\",\"issuer\":\"myissuer\"}");

        [Fact]
        public void ReadsAllFromJson()
            => ExpectObject(
                "{\"type\":\"mytype\",\"value\":\"myvalue\",\"valueType\":\"myvaluetype\",\"issuer\":\"myissuer\"}",
                new Claim(type: "mytype", value: "myvalue", valueType: "myvaluetype", issuer: "myissuer"));

        [Fact]
        public void HandlesDefaultValuesWhenReadingFromJson()
            => ExpectObject(
                "{\"type\":\"mytype\",\"value\":\"myvalue\"}",
                new Claim(type: "mytype", value: "myvalue"));
    }
}
