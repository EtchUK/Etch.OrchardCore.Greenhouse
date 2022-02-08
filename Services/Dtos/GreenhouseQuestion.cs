using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseQuestion
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fields")]
        public GreenhouseField[] Fields { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("required")]
        public bool? Required { get; set; }
    }
}
