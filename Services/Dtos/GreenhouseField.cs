using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("values")]
        public GreenhouseQuestionValue[] Values { get; set; }
    }
}
