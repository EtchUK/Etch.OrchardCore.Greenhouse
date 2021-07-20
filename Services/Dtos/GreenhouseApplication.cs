using Newtonsoft.Json;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseApplication
    {
        [JsonProperty("job_id")]
        public long JobId { get; set; }
    }
}
