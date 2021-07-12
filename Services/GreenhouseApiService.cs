using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Flurl.Http;
using OrchardCore.Entities;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhouseApiService : IGreenhouseApiService
    {
        #region Dependencies

        private readonly ISiteService _siteService;

        #endregion

        #region Constructor

        public GreenhouseApiService(ISiteService siteService)
        {
            _siteService = siteService;
        }

        #endregion

        #region Implementation

        public async Task<IEnumerable<GreenhouseJobPostingDto>> GetJobPostingsAsync(DateTime? updatedAfter)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/job_posts";

            if (updatedAfter.HasValue)
            {
                requestUrl += $"?updated_after={updatedAfter.Value.ToString("o")}";
            }

            return await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<IList<GreenhouseJobPostingDto>>();
        }

        #endregion
    }
}
