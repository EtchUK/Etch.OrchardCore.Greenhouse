using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobBoard
    {
        [JsonPropertyName("jobs")]
        public IList<GreenhouseJobBoardJob> Jobs { get; set; }
    }
}
