using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Services.Options;
using Flurl.Http;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<GreenhouseApiService> _logger;
        private readonly ISiteService _siteService;

        #endregion

        #region Constructor

        public GreenhouseApiService(ILogger<GreenhouseApiService> logger, ISiteService siteService)
        {
            _logger = logger;
            _siteService = siteService;
        }

        #endregion

        #region Implementation

        public async Task<GreenhouseApplicationResponse> CreateApplicationAsync(long jobId, GreenhouseApplication application)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"https://boards-api.greenhouse.io/v1/boards/{settings.BoardToken}/jobs/{jobId}";
            var response = new GreenhouseApplicationResponse();

            try
            {
                var rawResponse = await requestUrl
                    .WithBasicAuth(settings.ApiKey, string.Empty)
                    .PostJsonAsync(application.ToDictionary());

                response.ResponseCode = rawResponse.StatusCode;
            } 
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Exception thrown when attempting to submit job application");
                response.Error = ex.Message;
                response.ResponseCode = ex.StatusCode.HasValue ? ex.StatusCode.Value : 0;
            }

            return response;
        }

        public async Task<GreenhouseJob> GetJobAsync(long id)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/jobs/{id}";

            return await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<GreenhouseJob>();
        }

        public async Task<IList<GreenhouseJobPosting>> GetJobBoardAsync(string token)
        {
            var board = await $"https://boards-api.greenhouse.io/v1/boards/{token}/jobs".GetAsync().ReceiveJson<GreenhouseJobBoard>();
            var postings = new List<GreenhouseJobPosting>();

            foreach (var job in board.Jobs)
            {
                var posting = await GetJobPostingAsync(token, job.PostingId);

                // assume posting is active as it appears on job board
                posting.Active = posting.Live = true;
                postings.Add(posting);
            }

            return postings;
        }

        public async Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter, GreenhouseFilterOptions options, int page = 1)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var requestUrl = $"{settings.ApiHostname}/job_posts?active=true&live=true&per_page={PageSize}&page={page}";

            if (updatedAfter.HasValue)
            {
                requestUrl += $"&updated_after={updatedAfter.Value:o}";
            }

            var postings = await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<IList<GreenhouseJobPosting>>();

            if (postings.Count == PageSize)
            {
                postings.AddRange(await GetJobPostingsAsync(updatedAfter, options, page + 1));
            }

            return postings.Where(x => ShouldIncludePosting(x, options));
        }

        #endregion

        #region Private Methods

        private static async Task<GreenhouseJobPosting> GetJobPostingAsync(string token, long jobId)
        {
            return await $"https://boards-api.greenhouse.io/v1/boards/{token}/jobs/{jobId}?questions=true".GetAsync().ReceiveJson<GreenhouseJobPosting>();
        }

        private static async Task<GreenhouseJobPosting> GetJobPostingAsync(long id, GreenhouseSettings settings)
        {
            var requestUrl = $"{settings.ApiHostname}/job_posts/{id}";

            return await requestUrl.WithBasicAuth(settings.ApiKey, string.Empty).GetAsync().ReceiveJson<GreenhouseJobPosting>();
        }

        private bool ShouldIncludePosting(GreenhouseJobPosting posting, GreenhouseFilterOptions options)
        {
            if (options.Locations.Any() && (posting.Location == null || !options.Locations.Any(x => string.Equals(x, posting.Location.Name, StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            if (options.ExternalOnly && !posting.External)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
