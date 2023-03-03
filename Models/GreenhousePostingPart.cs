using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Lucene.Net.Documents;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
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
                ParsePostingData();

                if (_postingData?.Departments?.Any() ?? false)
                {
                    return _postingData?.Departments?.FirstOrDefault()?.Name;
                }

                ParseJobData();
                return _jobData?.Departments?.FirstOrDefault()?.Name;
            }
        }

        public long JobId
        {
            get
            {
                ParseJobData();
                return _jobData?.Id ?? 0;
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

        public long PostingId
        {
            get
            {
                ParsePostingData();
                return _postingData?.Id ?? 0;
            }
        }

        public DateTime? UpdatedAt
        {
            get
            {
                ParsePostingData();
                return _postingData?.UpdatedAt;
            }
        }

        public IList<GreenhouseMetadata> Metadata
        {
            get
            {
                ParsePostingData();
                return _postingData?.Metadata;
            }
        }

        #endregion

        #region Helper Methods

        public string GetMetadataValue(string propertyName)
        {
            var metadataField = Metadata.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (metadataField == null || metadataField.Value == null)
            {
                return string.Empty;
            }

            return string.Join(",", metadataField.Value);
        }

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
