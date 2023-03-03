using J2N.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseJob
    {
        [JsonProperty("confidential")]
        public bool Confidential { get; set; }

        [JsonProperty("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("departments")]
        public GreenhouseDepartment[] Departments { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_template")]
        public bool IsTemplate { get; set; }

        [JsonProperty("metadata")]
        public IList<GreenhouseMetadata> Metadata { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("offices")]
        public GreenhouseOffice[] Offices { get; set; }

        [JsonProperty("opened_at")]
        public DateTime OpenedAt { get; set; }

        [JsonProperty("statud")]
        public string Status { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
