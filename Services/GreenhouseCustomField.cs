using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhouseCustomField
    {
        [JsonProperty("name_key")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public JValue Value { get; set; }
    }
}
