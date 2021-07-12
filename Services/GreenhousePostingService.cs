using Etch.OrchardCore.Greenhouse.Indexes;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Newtonsoft.Json;
using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Liquid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhousePostingService : IGreenhousePostingService
    {
        #region Dependencies

        private readonly IContentManager _contentManager;
        private readonly ISession _session;
        private readonly ISlugService _slugService;

        #endregion

        #region Constructor

        public GreenhousePostingService(IContentManager contentManager, ISession session, ISlugService slugService)
        {
            _contentManager = contentManager;
            _session = session;
            _slugService = slugService;
        }

        #endregion

        #region Implementation

        public async Task<ContentItem> GetByGreenhouseIdAsync(long greenhouseId)
        {
            return await _session.Query<ContentItem, GreenhousePostingPartIndex>()
                .With<GreenhousePostingPartIndex>()
                .Where(x => x.GreenhouseId == greenhouseId)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTime?> GetLatestUpdatedAtAsync()
        {
            var contentItem = await _session.Query<ContentItem, GreenhousePostingPartIndex>()
                .With<GreenhousePostingPartIndex>()
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync();

            return contentItem != null ? contentItem.As<GreenhousePostingPart>().UpdateAt : (DateTime?)null;
        }

        public async Task SyncAsync(IList<GreenhouseJobPostingDto> postings)
        {
            foreach (var posting in postings)
            {
                await SyncAsync(posting);
            }
        }

        #endregion

        #region Helpers

        private async Task CreateAsync(GreenhouseJobPostingDto posting)
        {
            var contentItem = await _contentManager.NewAsync(Constants.Definitions.ContentType);
            contentItem.DisplayText = posting.Title;

            var greenhousePostingPart = contentItem.As<GreenhousePostingPart>();
            greenhousePostingPart.GreenhouseId = posting.Id;
            greenhousePostingPart.Data = JsonConvert.SerializeObject(posting);
            contentItem.Apply(nameof(GreenhousePostingPart), greenhousePostingPart);

            var autoroutePart = contentItem.As<AutoroutePart>();
            autoroutePart.Path = $"{_slugService.Slugify(posting.Title)}/{posting.Id}";
            contentItem.Apply(nameof(AutoroutePart), autoroutePart);

            ContentExtensions.Apply(contentItem, contentItem);

            await _contentManager.CreateAsync(contentItem);

            if (posting.Live)
            {
                await _contentManager.PublishAsync(contentItem);
            }
        }

        private async Task SyncAsync(GreenhouseJobPostingDto posting)
        {
            var contentItem = await GetByGreenhouseIdAsync(posting.Id);

            if (contentItem == null)
            {
                await CreateAsync(posting);
                return;
            }

            await UpdateAsync(contentItem, posting);
        }

        private async Task UpdateAsync(ContentItem contentItem, GreenhouseJobPostingDto posting)
        {
            contentItem.DisplayText = posting.Title;

            var greenhousePostingPart = contentItem.As<GreenhousePostingPart>();
            greenhousePostingPart.GreenhouseId = posting.Id;
            greenhousePostingPart.Data = JsonConvert.SerializeObject(posting);
            contentItem.Apply(nameof(GreenhousePostingPart), greenhousePostingPart);

            ContentExtensions.Apply(contentItem, contentItem);

            await _contentManager.UpdateAsync(contentItem);

            if (posting.Live)
            {
                await _contentManager.PublishAsync(contentItem);
            }
        }

        #endregion
    }
}
