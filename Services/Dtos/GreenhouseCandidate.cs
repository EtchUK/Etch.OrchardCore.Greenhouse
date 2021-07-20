using Newtonsoft.Json;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseCandidate : GreenhouseCandidateBase
    {
        [JsonProperty("custom_fields")]
        public IList<GreenhouseCustomField> CustomFields { get; set; } = new List<GreenhouseCustomField>();
    }
}
