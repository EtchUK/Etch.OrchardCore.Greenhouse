using Etch.OrchardCore.Greenhouse.Models;
using OrchardCore.Indexing;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Indexes
{
    public class GreenhousePostingPartIndexHandler : ContentPartIndexHandler<GreenhousePostingPart>
    {
        public override Task BuildIndexAsync(GreenhousePostingPart part, BuildPartIndexContext context)
        {
            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Department)}", part.Department, DocumentIndexOptions.Store);
            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Location)}", part.Location, DocumentIndexOptions.Store);
            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.UpdatedAt)}", part.UpdatedAt, DocumentIndexOptions.Store);

            return Task.CompletedTask;
        }
    }
}
