using Etch.OrchardCore.Greenhouse.Models;
using Fluid;
using Fluid.Values;
using OrchardCore.ContentManagement;
using OrchardCore.Liquid;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Filters
{
    public class DisplayMetadataFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var contentItem = input.ToObjectValue() as ContentItem;
            var propertyName = ((StringValue)arguments["property"]).ToObjectValue().ToString();

            if (contentItem == null)
            {
                return NilValue.Empty;
            }

            var greenhousePostingPart = contentItem.As<GreenhousePostingPart>();

            if (greenhousePostingPart == null)
            {
                return NilValue.Empty;
            }

            return new ValueTask<FluidValue>(new StringValue(greenhousePostingPart.GetMetadataValue(propertyName)));
        }
    }
}
