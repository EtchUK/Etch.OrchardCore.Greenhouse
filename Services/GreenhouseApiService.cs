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

        public async Task<IList<GreenhouseAttachmentResponse>> AddAttachmentsAsync(long candidateId, IList<GreenhouseAttachment> attachments)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var attachmentResponses = new List<GreenhouseAttachmentResponse>();
            var requestUrl = $"{settings.ApiHostname}/candidates/{candidateId}/attachments";

            foreach (var attachment in attachments)
            {
                attachmentResponses.Add(await (await requestUrl
                    .WithHeader("On-Behalf-Of", settings.OnBehalfOfId)
                    .WithBasicAuth(settings.ApiKey, string.Empty)
                    .PostJsonAsync(attachment))
                .GetJsonAsync<GreenhouseAttachmentResponse>());
            }

            return attachmentResponses;
        }

        public async Task<GreenhouseCandidateResponse> CreateCandidateAsync(GreenhouseCandidate candidate)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/candidates";

            return await (await requestUrl
                .WithHeader("On-Behalf-Of", settings.OnBehalfOfId)
                .WithBasicAuth(settings.ApiKey, string.Empty)
                .PostJsonAsync(candidate))
            .GetJsonAsync<GreenhouseCandidateResponse>();
        }

        public async Task<GreenhouseJob> GetJobAsync(long id)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/jobs/{id}";

            return await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<GreenhouseJob>();
        }

        public async Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/job_posts";

            if (updatedAfter.HasValue)
            {
                requestUrl += $"?updated_after={updatedAfter.Value:o}";
            }

            return await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<IList<GreenhouseJobPosting>>();
        }

        #endregion
    }
}
