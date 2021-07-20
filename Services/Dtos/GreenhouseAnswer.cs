using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseAnswer
    {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }
    }
}
