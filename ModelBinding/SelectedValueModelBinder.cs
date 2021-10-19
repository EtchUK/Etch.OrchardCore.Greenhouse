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
            modelState.SetModelValue(question.Name, request.Form[question.Name], request.Form[question.Name]);

            if (question.Required.HasValue && question.Required.Value && string.IsNullOrWhiteSpace(request.Form[question.Name]))
            {
                modelState.AddModelError(question.Name, $"{question.Label} is required");
            }

            if (!string.IsNullOrWhiteSpace(request.Form[question.Name]) && question.Values.Any() && !question.Values.Any(x => x.Value.ToString() == request.Form[question.Name].ToString()))
            {
                modelState.AddModelError(question.Name, $"Unrecognised value: {request.Form[question.Name]}");
            }

            if (modelState[question.Name].ValidationState != ModelValidationState.Invalid)
            {
                modelState.MarkFieldValid(question.Name);
            }

            if (string.IsNullOrEmpty(request.Form[question.Name]))
            {
                return null;
            }

            return question.Values.SingleOrDefault(x => x.Value.ToString() == request.Form[question.Name].ToString())?.Value ?? null;
        }
    }
}
