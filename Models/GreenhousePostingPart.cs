using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using System;

namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhousePostingPart : ContentPart
    {
        private GreenhouseJobPosting _postingData { get; set; }

        public string JobData { get; set; }
        public long GreenhouseId { get; set; }
        public bool IgnoreSync { get; set; }
        public string PostingData { get; set; }

        public DateTime? UpdateAt
        {
            get
            {
                if (string.IsNullOrEmpty(PostingData))
                {
                    return null;
                }

                if (_postingData == null)
                {
                    _postingData = JsonConvert.DeserializeObject<GreenhouseJobPosting>(PostingData);
                }

                return _postingData.UpdatedAt;
            }
        }
    }
}
