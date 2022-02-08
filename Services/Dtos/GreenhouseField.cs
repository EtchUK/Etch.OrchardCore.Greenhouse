using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseField
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("values")]
        public GreenhouseQuestionValue[] Values { get; set; }
    }
}
