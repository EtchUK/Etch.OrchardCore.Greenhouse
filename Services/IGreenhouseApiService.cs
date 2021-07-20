using Etch.OrchardCore.Greenhouse.Services.Dtos;
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
        Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter);
    }
}
