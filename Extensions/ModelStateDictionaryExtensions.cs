using Etch.OrchardCore.Greenhouse.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static string Serialize(this ModelStateDictionary modelState)
        {
            return JsonConvert.SerializeObject(modelState
                .Select(x => new ModelStateValueDto
                {
                    Key = x.Key,
                    AttemptedValue = x.Value.AttemptedValue,
                    RawValue = x.Value.RawValue,
                    ErrorMessages = x.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                }));
        }

        public static ModelStateDictionary DeserializeModelState(this string value)
        {
            var modelState = new ModelStateDictionary();

            if (string.IsNullOrEmpty(value))
            {
                return modelState;
            }

            foreach (var item in JsonConvert.DeserializeObject<List<ModelStateValueDto>>(value))
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);

                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }

            return modelState;
        }
    }
}
