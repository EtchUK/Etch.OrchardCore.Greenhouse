using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJob
    {
        [JsonPropertyName("confidential")]
        public bool Confidential { get; set; }

        [JsonPropertyName("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("departments")]
        public GreenhouseDepartment[] Departments { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("is_template")]
        public bool IsTemplate { get; set; }

        [JsonPropertyName("metadata")]
        public IList<GreenhouseMetadata> Metadata { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("offices")]
        public GreenhouseOffice[] Offices { get; set; }

        [JsonPropertyName("opened_at")]
        public DateTime OpenedAt { get; set; }

        [JsonPropertyName("statud")]
        public string Status { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
