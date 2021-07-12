using Etch.OrchardCore.Greenhouse.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public interface IGreenhouseApiService
    {
        Task<IEnumerable<GreenhouseJobPostingDto>> GetJobPostingsAsync(DateTime? updatedAfter);
    }
}
