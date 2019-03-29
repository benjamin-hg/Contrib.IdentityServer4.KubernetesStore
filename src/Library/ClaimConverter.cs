using System;
using System.Security.Claims;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contrib.IdentityServer4.KubernetesStore
{
    /// <summary>
    /// Converts <see cref="Claim"/>s to and from JSON.
    /// </summary>
    public class ClaimConverter : JsonConverter<Claim>
    {
        public override void WriteJson(JsonWriter writer, Claim value, JsonSerializer serializer)
        {
            new JObject
            {
                ["type"] = value.Type,
                ["value"] = value.Value,
                ["valueType"] = value.ValueType,
                ["issuer"] = value.Issuer
            }.WriteTo(writer);
        }

        public override Claim ReadJson(JsonReader reader, Type objectType, Claim existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Claim(
                obj["type"].Value<string>(),
                obj["value"].Value<string>(),
                obj.TryGetString("valueType") ?? "http://www.w3.org/2001/XMLSchema#string",
                obj.TryGetString("issuer") ?? "LOCAL AUTHORITY");
        }
    }
}
