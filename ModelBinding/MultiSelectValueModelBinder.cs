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
            modelState.SetModelValue(question.Name, request.Form[question.Name], request.Form[question.Name]);

            if (question.Required && string.IsNullOrWhiteSpace(request.Form[question.Name]))
            {
                modelState.AddModelError(question.Name, $"{question.Label} is required");
            }

            foreach (var selectedValue in request.Form[question.Name].ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
            {
                if (question.Values.Any() && !question.Values.Any(x => selectedValue.Equals(x.Value.ToString())))
                {
                    modelState.AddModelError(question.Name, $"Unrecognised value: {selectedValue}");
                }
            }

            if (modelState[question.Name].ValidationState != ModelValidationState.Invalid)
            {
                modelState.MarkFieldValid(question.Name);
            }

            if (string.IsNullOrEmpty(request.Form[question.Name]))
            {
                return null;
            }

            return request.Form[question.Name].ToString();
        }
    }
}

