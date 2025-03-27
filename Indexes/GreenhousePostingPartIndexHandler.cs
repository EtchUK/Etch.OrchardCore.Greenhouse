using Etch.OrchardCore.Greenhouse.Models;
using OrchardCore.Indexing;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Indexes
{
    public class GreenhousePostingPartIndexHandler : ContentPartIndexHandler<GreenhousePostingPart>
    {
        public override Task BuildIndexAsync(GreenhousePostingPart part, BuildPartIndexContext context)
        {
            var options = context.Settings.ToOptions();

            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Department)}", part.Department, options);
            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Location)}", part.Location, options);
            context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.UpdatedAt)}", part.UpdatedAt, options);

            foreach (var metadataField in part.GetMetadata())
            {
                if (metadataField.Value?.Any() ?? false)
                {
                    context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.Metadata.{FormatMetadataName(metadataField.Name)}", metadataField.Value[0], options);
                }
                else
                {
                    context.DocumentIndex.Set($"{nameof(GreenhousePostingPart)}.Metadata.{FormatMetadataName(metadataField.Name)}", "NULL", options);
                }
            }

            return Task.CompletedTask;
        }

        private static string FormatMetadataName(string name) => name.Replace(" ", "").Replace("-", "");
    }
}
