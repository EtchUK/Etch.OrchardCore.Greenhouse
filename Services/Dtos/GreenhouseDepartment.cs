using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseDepartment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
