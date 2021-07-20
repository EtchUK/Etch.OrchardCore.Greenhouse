using Newtonsoft.Json;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public abstract class GreenhouseCandidateBase
    {
        [JsonProperty("applications")]
        public IList<GreenhouseApplication> Applications { get; set; } = new List<GreenhouseApplication>();

        [JsonIgnore]
        public IList<GreenhouseAttachment> Attachments { get; set; } = new List<GreenhouseAttachment>();

        [JsonProperty("email_addresses")]
        public IList<GreenhouseValueType> EmailAddresses { get; set; } = new List<GreenhouseValueType>();

        [JsonProperty("first_name")]
        public string Firstname { get; set; }

        [JsonProperty("last_name")]
        public string Lastname { get; set; }

        [JsonProperty("phone_numbers")]
        public IList<GreenhouseValueType> PhoneNumbers { get; set; } = new List<GreenhouseValueType>();
    }
}
