using Etch.OrchardCore.Greenhouse.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public interface IGreenhouseApiService
    {
        Task<IEnumerable<GreenhouseJobPosting>> GetJobPostingsAsync(DateTime? updatedAfter);
        Task<GreenhouseJob> GetJobAsync(long id);
    }
}
