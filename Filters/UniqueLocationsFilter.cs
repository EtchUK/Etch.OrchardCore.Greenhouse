using Etch.OrchardCore.Greenhouse.Models;
using Fluid;
using Fluid.Values;
using OrchardCore.Liquid;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Filters
{
    public class UniqueLocationsFilter : ILiquidFilter
    {
        public async ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var locations = new List<string>();

            foreach (var value in input.Enumerate())
            {
                locations.Add((await value.GetValueAsync($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Location)}", context)).ToStringValue());
            }

            return new ArrayValue(locations.Distinct().OrderBy(x => x).Select(x => FluidValue.Create(x, context.Options)).ToArray());
        }
    }
}
