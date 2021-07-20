using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using System;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhousePostingPart : ContentPart
    {
        private GreenhouseJob _jobData { get; set; }
        private GreenhouseJobPosting _postingData { get; set; }

        public string JobData { get; set; }
        public long GreenhouseId { get; set; }
        public bool IgnoreSync { get; set; }
        public string PostingData { get; set; }

        #region Index Properties

        public string Department
        {
            get
            {
                ParseJobData();
                return _jobData?.Departments?.FirstOrDefault()?.Name;
            }
        }

        public string Location
        {
            get
            {
                ParsePostingData();
                return _postingData?.Location?.Name;
            }
        }

        public DateTime? UpdateAt
        {
            get
            {
                ParsePostingData();
                return _postingData?.UpdatedAt;
            }
        }

        #endregion

        #region Helper Methods

        public GreenhouseJobPosting GetJobPostingData()
        {
            return JsonConvert.DeserializeObject<GreenhouseJobPosting>(PostingData);
        }

        private void ParseJobData()
        {
            if (string.IsNullOrEmpty(JobData))
            {
                return;
            }

            if (_jobData == null)
            {
                _jobData = JsonConvert.DeserializeObject<GreenhouseJob>(JobData);
            }
        }

        private void ParsePostingData()
        {
            if (string.IsNullOrEmpty(PostingData))
            {
                return;
            }

            if (_postingData == null)
            {
                _postingData = JsonConvert.DeserializeObject<GreenhouseJobPosting>(PostingData);
            }
        }

        #endregion
    }
}
