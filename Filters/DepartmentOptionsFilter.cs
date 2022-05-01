using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Utilities;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Http;
using OrchardCore.Liquid;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Filters
{
    public class DepartmentOptionsFilter : ILiquidFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentOptionsFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var departments = new List<string>();

            foreach (var value in input.Enumerate(context))
            {
                departments.Add((await value.GetValueAsync($"{nameof(GreenhousePostingPart)}.{nameof(GreenhousePostingPart.Department)}", context)).ToStringValue());
            }

            return new StringValue(StringUtils.GetOptions(departments.Distinct().OrderBy(x => x).ToList(), _httpContextAccessor.HttpContext.Request.Query["department"].FirstOrDefault()));
        }
    }
}
