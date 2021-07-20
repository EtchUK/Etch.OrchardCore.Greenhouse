using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class TextModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            modelState.SetModelValue(question.Name, request.Form[question.Name], request.Form[question.Name]);

            if (question.Required && string.IsNullOrWhiteSpace(request.Form[question.Name]))
            {
                modelState.AddModelError(question.Name, $"{question.Label} is required");
                return request.Form[question.Name].ToString();
            }

            modelState.MarkFieldValid(question.Name);
            return request.Form[question.Name].ToString();
        }
    }
}
