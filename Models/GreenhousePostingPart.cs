using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using System;

namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhousePostingPart : ContentPart
    {
        private GreenhouseJobPostingDto _data { get; set; }

        public string Data { get; set; }
        public long GreenhouseId { get; set; }
        public bool IgnoreSync { get; set; }

        public DateTime? UpdateAt
        {
            get
            {
                if (string.IsNullOrEmpty(Data))
                {
                    return null;
                }

                if (_data == null)
                {
                    _data = JsonConvert.DeserializeObject<GreenhouseJobPostingDto>(Data);
                }

                return _data.UpdatedAt;
            }
        }
    }
}
