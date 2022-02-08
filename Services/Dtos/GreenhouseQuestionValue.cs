using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseQuestionValue
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}
