using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseValueType
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
