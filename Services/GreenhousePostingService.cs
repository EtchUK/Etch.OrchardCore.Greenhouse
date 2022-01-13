using Etch.OrchardCore.Greenhouse.Indexes;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Services.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Infrastructure.Html;
using OrchardCore.Liquid;
using OrchardCore.Title.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using YesSql;
using YesSql.Services;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhousePostingService : IGreenhousePostingService
    {
        #region Dependencies

        private readonly IContentManager _contentManager;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IGreenhouseApiService _greenhouseApiService;
        private readonly ILogger<GreenhousePostingService> _logger;
        private readonly ISession _session;
        private readonly ISlugService _slugService;

        private readonly Dictionary<long, GreenhouseJob> _cachedJobs = new Dictionary<long, GreenhouseJob>();

        #endregion

        #region Constructor

        public GreenhousePostingService(IContentManager contentManager, HtmlEncoder htmlEncoder, IGreenhouseApiService greenhouseApiService, ILogger<GreenhousePostingService> logger, ISession session, ISlugService slugService)
        {
            _contentManager = contentManager;
            _htmlEncoder = htmlEncoder;
            _greenhouseApiService = greenhouseApiService;
            _logger = logger;
            _session = session;
            _slugService = slugService;
        }

        #endregion

        #region Implementation

        public async Task<ContentItem> GetByGreenhouseIdAsync(long greenhouseId)
        {
            return await _session.Query<ContentItem>()
                .With<ContentItemIndex>()
                    .Where(x => x.Latest)
                .With<GreenhousePostingPartIndex>()
                    .Where(x => x.GreenhouseId == greenhouseId)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTime?> GetLatestUpdatedAtAsync()
        {
            var contentItem = await _session.Query<ContentItem>()
                .With<ContentItemIndex>()
                    .Where(x => x.Latest)
                .With<GreenhousePostingPartIndex>()
                    .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync();

            return contentItem != null ? contentItem.As<GreenhousePostingPart>().UpdatedAt.Value.AddSeconds(1) : (DateTime?)null;
        }

        public async Task SyncAsync(IList<GreenhouseJobPosting> postings, GreenhouseSyncOptions options)
        {
            foreach (var posting in postings)
            {
                try
                {
                    await SyncAsync(posting, options);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to sync posting: {posting.Id}");
                }
            }

            await RemoveDeletedPostsAsync(postings);
        }

        public async Task RemoveDeletedPostsAsync(IList<GreenhouseJobPosting> postings)
        {
            var postingIds = postings.Select(x => x.Id).ToArray();
            var contentItemsToBeRemoved = await _session.Query<ContentItem>()
                .With<ContentItemIndex>()
                    .Where(x => x.Latest)
                .With<GreenhousePostingPartIndex>()
                    .Where(x => x.GreenhouseId.IsNotIn(postingIds))
                .ListAsync();

            foreach (var contentItem in contentItemsToBeRemoved)
            {
                await _contentManager.RemoveAsync(contentItem);
            }
        }

        #endregion

        #region Helpers

        private async Task CreateAsync(GreenhouseJobPosting posting, GreenhouseJob job, GreenhouseSyncOptions options)
        {
            var contentItem = await _contentManager.NewAsync(options.ContentType);
            contentItem.DisplayText = posting.Title;

            var encoded = _htmlEncoder.Encode(posting.Content);

            var greenhousePostingPart = contentItem.As<GreenhousePostingPart>();
            greenhousePostingPart.GreenhouseId = posting.Id;
            greenhousePostingPart.JobData = JsonConvert.SerializeObject(job);
            greenhousePostingPart.PostingData = JsonConvert.SerializeObject(posting);
            contentItem.Apply(nameof(GreenhousePostingPart), greenhousePostingPart);

            var autoroutePart = contentItem.As<AutoroutePart>();
            autoroutePart.Path = $"{options.UrlPrefix ?? string.Empty}{_slugService.Slugify(posting.Title)}/{posting.Id}";
            contentItem.Apply(nameof(AutoroutePart), autoroutePart);

            var titlePart = contentItem.As<TitlePart>();
            titlePart.Title = posting.Title;
            contentItem.Apply(nameof(TitlePart), titlePart);

            contentItem.Author = options.Author;

            ContentExtensions.Apply(contentItem, contentItem);

            await _contentManager.CreateAsync(contentItem);

            if (posting.Live)
            {
                await _contentManager.PublishAsync(contentItem);
            }
        }

        private async Task<bool> CheckJobExists(long id)
        {
            return await _session.Query<ContentItem>()
                .With<GreenhousePostingPartIndex>()
                    .Where(x => x.JobId == id)
                .With<ContentItemIndex>()
                    .Where(x => x.Latest)
                .CountAsync() > 0;
        }

        private async Task RemoveAsync(ContentItem contentItem)
        {
            await _contentManager.RemoveAsync(contentItem);
        }

        private async Task SyncAsync(GreenhouseJobPosting posting, GreenhouseSyncOptions options)
        {
            var contentItem = await GetByGreenhouseIdAsync(posting.Id);

            if ((contentItem != null && (contentItem.As<GreenhousePostingPart>()?.IgnoreSync ?? false)) || (contentItem == null && !posting.Active))
            {
                return;
            }

            if (options.PreventDuplicatePostingsForSameJob && contentItem == null && await CheckJobExists(posting.JobId)) {
                return;
            }

            if (!_cachedJobs.ContainsKey(posting.JobId))
            {
                _cachedJobs.Add(posting.JobId, await _greenhouseApiService.GetJobAsync(posting.JobId));
            }

            var job = _cachedJobs[posting.JobId];

            if (contentItem == null)
            {
                await CreateAsync(posting, job, options);
                return;
            }

            if (!posting.Active)
            {
                await RemoveAsync(contentItem);
                return;
            }

            await UpdateAsync(contentItem, posting, job, options);
        }

        private async Task UpdateAsync(ContentItem contentItem, GreenhouseJobPosting posting, GreenhouseJob job, GreenhouseSyncOptions options)
        {
            contentItem.DisplayText = posting.Title;

            var greenhousePostingPart = contentItem.As<GreenhousePostingPart>();
            greenhousePostingPart.GreenhouseId = posting.Id;
            greenhousePostingPart.JobData = JsonConvert.SerializeObject(job);
            greenhousePostingPart.PostingData = JsonConvert.SerializeObject(posting);
            contentItem.Apply(nameof(GreenhousePostingPart), greenhousePostingPart);

            contentItem.Author = options.Author;

            ContentExtensions.Apply(contentItem, contentItem);

            await _contentManager.UpdateAsync(contentItem);

            if (!posting.Live)
            {
                await _contentManager.UnpublishAsync(contentItem);
                return;
            }

            await _contentManager.PublishAsync(contentItem);
        }

        #endregion
    }
}
