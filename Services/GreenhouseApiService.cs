using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Flurl.Http;
using OrchardCore.Entities;
using OrchardCore.Settings;
using OrchardCore.Workflows.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhouseApiService : IGreenhouseApiService
    {
        #region Constants

        private const int PageSize = 150;

        #endregion

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

        public async Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter, int page = 1)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/job_posts?active=true&live=true&per_page={PageSize}&page={page}";

            if (updatedAfter.HasValue)
            {
                requestUrl += $"?updated_after={updatedAfter.Value:o}";
            }

            var postings = await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<IList<GreenhouseJobPosting>>();

            if (postings.Count == PageSize)
            {
                postings.AddRange(await GetJobPostingsAsync(updatedAfter, page + 1));
            }

            return postings.Where(x => x.External);
        }

        #endregion
    }
}
