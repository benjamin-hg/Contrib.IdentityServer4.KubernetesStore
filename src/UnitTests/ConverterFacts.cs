using System.Security.Claims;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public abstract class ConverterFacts<T>
    {
        private readonly JsonSerializerSettings _serializerSettings;

        protected ConverterFacts(JsonConverter<T> converter)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = {converter}
            };
        }

        protected void ExpectJson(T obj, string expectedJson)
            => JsonConvert.SerializeObject(obj, _serializerSettings)
                          .Should().Be(expectedJson);

        protected void ExpectObject(string json, T expectedObj)
            => JsonConvert.DeserializeObject<Claim>(json, _serializerSettings)
                          .Should().BeEquivalentTo(expectedObj);
    }
}
