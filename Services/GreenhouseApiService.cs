using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using OrchardCore.Entities;
using OrchardCore.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhouseApiService : IGreenhouseApiService
    {
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

        public async Task<IList<GreenhouseJobPosting>> GetJobBoardAsync()
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();
            var board = await $"https://boards-api.greenhouse.io/v1/boards/{settings.BoardToken}/jobs".GetAsync().ReceiveJson<GreenhouseJobBoard>();
            var postings = new List<GreenhouseJobPosting>();

            foreach (var job in board.Jobs)
            {
                var posting = await GetJobPostingAsync(settings.BoardToken, job.PostingId);

                // assume posting is active as it appears on job board
                posting.Active = posting.Live = true;
                postings.Add(posting);
            }

            return postings;
        }

        #endregion

        #region Private Methods

        private static async Task<GreenhouseJobPosting> GetJobPostingAsync(string token, long jobId)
        {
            return await $"https://boards-api.greenhouse.io/v1/boards/{token}/jobs/{jobId}?questions=true".GetAsync().ReceiveJson<GreenhouseJobPosting>();
        }

        #endregion
    }
}
