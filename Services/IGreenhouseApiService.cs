using Etch.OrchardCore.Greenhouse.Services.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public interface IGreenhouseApiService
    {
        Task<GreenhouseApplicationResponse> CreateApplicationAsync(long jobId, GreenhouseApplication application);
        Task<IList<GreenhouseJobPosting>> GetJobBoardAsync();
    }
}
