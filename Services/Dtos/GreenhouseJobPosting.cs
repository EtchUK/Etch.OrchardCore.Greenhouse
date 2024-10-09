using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJobPosting
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("departments")]
        public GreenhouseDepartment[] Departments { get; set; }

        [JsonPropertyName("external")]
        public bool External { get; set; }

        [JsonPropertyName("first_published_at")]
        public DateTime? FirstPublishedAt { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("internal")]
        public bool Internal { get; set; }

        [JsonPropertyName("internal_content")]
        public string InternalContent { get; set; }

        [JsonPropertyName("job_id")]
        public long JobId { get; set; }

        [JsonPropertyName("live")]
        public bool Live { get; set; }

        [JsonPropertyName("location")]
        public GreenhouseLocation Location { get; set; }

        [JsonPropertyName("metadata")]
        public IList<GreenhouseMetadata> Metadata { get; set; }

        [JsonPropertyName("questions")]
        public IList<GreenhouseQuestion> Questions { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
