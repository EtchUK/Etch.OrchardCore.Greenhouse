using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class SelectedValueModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            var field = question.Fields.FirstOrDefault();

            modelState.SetModelValue(field.Name, request.Form[field.Name], request.Form[field.Name]);

            if (question.Required.HasValue && question.Required.Value && string.IsNullOrWhiteSpace(request.Form[field.Name]))
            {
                modelState.AddModelError(field.Name, $"{question.Label} is required");
            }

            if (!string.IsNullOrWhiteSpace(request.Form[field.Name]) && field.Values.Any() && !field.Values.Any(x => x.Value.ToString() == request.Form[field.Name].ToString()))
            {
                modelState.AddModelError(field.Name, $"Unrecognised value: {request.Form[field.Name]}");
            }

            if (modelState[field.Name].ValidationState != ModelValidationState.Invalid)
            {
                modelState.MarkFieldValid(field.Name);
            }

            if (string.IsNullOrEmpty(request.Form[field.Name]))
            {
                return null;
            }

            return field.Values.SingleOrDefault(x => x.Value.ToString() == request.Form[field.Name].ToString())?.Value ?? null;
        }
    }
}
