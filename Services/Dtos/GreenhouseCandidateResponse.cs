using Newtonsoft.Json;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseCandidateResponse : GreenhouseCandidateBase
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("custom_fields")]
        public IDictionary<string, object> CustomFields { get; set; }
    }
}
