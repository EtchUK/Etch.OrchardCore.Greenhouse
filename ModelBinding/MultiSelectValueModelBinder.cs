using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class MultiSelectValueModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            var field = question.Fields.FirstOrDefault();

            modelState.SetModelValue(field.Name, request.Form[field.Name], request.Form[field.Name]);

            if (question.Required.HasValue && question.Required.Value && string.IsNullOrWhiteSpace(request.Form[field.Name]))
            {
                modelState.AddModelError(field.Name, $"{question.Label} is required");
            }

            foreach (var selectedValue in request.Form[field.Name].ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
            {
                if (field.Values.Any() && !field.Values.Any(x => selectedValue.Equals(x.Value.ToString())))
                {
                    modelState.AddModelError(field.Name, $"Unrecognised value: {selectedValue}");
                }
            }

            if (modelState[field.Name].ValidationState != ModelValidationState.Invalid)
            {
                modelState.MarkFieldValid(field.Name);
            }

            if (string.IsNullOrEmpty(request.Form[field.Name]))
            {
                return null;
            }

            return request.Form[field.Name].ToString();
        }
    }
}

