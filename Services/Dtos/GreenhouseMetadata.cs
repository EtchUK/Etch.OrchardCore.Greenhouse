using Etch.OrchardCore.Greenhouse.Converters;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseMetadata
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        [JsonConverter(typeof(StringOrArrayConverter))]
        public string[] Value { get; set; }

        [JsonPropertyName("value_type")]
        public string ValueType { get; set; }
    }
}
