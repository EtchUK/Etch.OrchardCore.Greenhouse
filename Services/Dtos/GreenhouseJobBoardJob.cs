using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobBoardJob
    {
        [JsonPropertyName("absolute_url")]
        public string AbsoluteUrl { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("internal_job_id")]
        public long InternalJobId { get; set; }

        [JsonPropertyName("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public long PostingId
        {
            get { return long.Parse(AbsoluteUrl.Split("/").LastOrDefault()); }
        }
    }
}
