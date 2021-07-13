using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseOffice
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
