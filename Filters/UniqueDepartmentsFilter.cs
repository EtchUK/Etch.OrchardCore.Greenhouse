using Etch.OrchardCore.Greenhouse.Models;
using Fluid;
using Fluid.Values;
using OrchardCore.Liquid;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Filters
{
    public class UniqueDepartmentsFilter : ILiquidFilter
    {
        public async ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var departments = new List<string>();

            foreach (var value in input.Enumerate())
            {
                departments.Add((await value.GetValueAsync($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Department)}", context)).ToStringValue());
            }

            return new ArrayValue(departments.Distinct().OrderBy(x => x).Select(x => FluidValue.Create(x, context.Options)).ToArray());
        }
    }
}
