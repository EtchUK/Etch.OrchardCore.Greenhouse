using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class AttachmentModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            if (question.Required && !request.Form.Files.Any(x => x.Name == question.Name))
            {
                modelState.AddModelError(question.Name, $"{question.Label} is required");
                return null;
            }

            modelState.MarkFieldValid(question.Name);

            if (!request.Form.Files.Any(x => x.Name == question.Name))
            {
                return null;
            }

            return new GreenhouseAttachment {
                Content = request.Form.Files[question.Name].ToBase64(),
                ContentType = request.Form.Files[question.Name].ContentType,
                Filename = request.Form.Files[question.Name].FileName,
                Type = question.Name
            };
        }
    }
}
