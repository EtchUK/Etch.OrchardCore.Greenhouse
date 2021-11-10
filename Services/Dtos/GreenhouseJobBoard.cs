using Newtonsoft.Json;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobBoard
    {
        [JsonProperty("jobs")]
        public IList<GreenhouseJobBoardJob> Jobs { get; set; }
    }
}
