using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Services.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public interface IGreenhouseApiService
    {
        Task<IList<GreenhouseAttachmentResponse>> AddAttachmentsAsync(long candidateId, IList<GreenhouseAttachment> attachments);
        Task<GreenhouseCandidateResponse> CreateCandidateAsync(GreenhouseCandidate candidate);
        Task<GreenhouseJob> GetJobAsync(long id);
        Task<IList<GreenhouseJobPosting>> GetJobBoardAsync(string token);
        Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter, GreenhouseFilterOptions options, int page = 1);
    }
}
