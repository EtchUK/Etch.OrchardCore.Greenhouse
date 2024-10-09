using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseQuestion
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("fields")]
        public GreenhouseField[] Fields { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("required")]
        public bool? Required { get; set; }
    }
}
