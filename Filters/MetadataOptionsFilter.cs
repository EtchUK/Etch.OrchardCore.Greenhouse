using AngleSharp.Css;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Utilities;
using Fluid;
using Fluid.Values;
using OrchardCore.Liquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Filters
{
    public class MetadataOptionsFilter : ILiquidFilter
    {
        public async ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var metadataField = new List<string>();
            var propertyName = arguments["property"].ToStringValue();

            foreach (var value in input.Enumerate(context))
            {
                if (!string.IsNullOrEmpty((await value.GetValueAsync($"{nameof(GreenhousePostingPart)}.Metadata.{propertyName}", context)).ToStringValue()))
                {
                    metadataField.Add((await value.GetValueAsync($"{nameof(GreenhousePostingPart)}.Metadata.{propertyName}", context)).ToStringValue());
                }
            }

            return new StringValue(StringUtils.GetOptions(metadataField.Distinct().OrderBy(x => x).ToList(), arguments["selectedItem"].ToStringValue()));
        }
    }
}