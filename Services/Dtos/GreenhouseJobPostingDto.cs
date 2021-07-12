using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobPostingDto
    {
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("external")]
        public bool External { get; set; }

        [JsonProperty("first_published_at")]
        public DateTime FirstPublishedAt { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("internal")]
        public bool Internal { get; set; }

        [JsonProperty("internal_content")]
        public string InternalContent { get; set; }

        [JsonProperty("job_id")]
        public long JobId { get; set; }

        [JsonProperty("live")]
        public bool Live { get; set; }

        [JsonProperty("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonProperty("questions")]
        public IList<GreenhouseQuestion> Questions { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
