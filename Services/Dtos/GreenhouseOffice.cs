using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseOffice
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
