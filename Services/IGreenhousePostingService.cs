using Etch.OrchardCore.Greenhouse.Services.Dtos;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public interface IGreenhousePostingService
    {
        Task<ContentItem> GetByGreenhouseIdAsync(long greenhouseId);
        Task<DateTime?> GetLatestUpdatedAtAsync();
        Task SyncAsync(IList<GreenhouseJobPostingDto> postings);
    }
}
