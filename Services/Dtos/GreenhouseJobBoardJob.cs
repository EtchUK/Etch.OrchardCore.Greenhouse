using Newtonsoft.Json;
using System;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobBoardJob
    {
        [JsonProperty("absolute_url")]
        public string AbsoluteUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("internal_job_id")]
        public long InternalJobId { get; set; }

        [JsonProperty("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public long PostingId
        {
            get { return long.Parse(AbsoluteUrl.Split("/").LastOrDefault()); }
        }
    }
}
