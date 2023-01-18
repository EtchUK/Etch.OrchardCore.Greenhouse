using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class AttachmentTextModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            if (question == null)
            {
                return null;
            }

            var fileField = question.Fields.FirstOrDefault(x => x.Type == Constants.GreenhouseFieldTypes.Attachment);
            var textField = question.Fields.FirstOrDefault(x => x.Type == Constants.GreenhouseFieldTypes.LongText);

            if (fileField == null || textField == null)
            {
                return null;
            }

            if (question.Required.HasValue && question.Required.Value && !request.Form.Files.Any(x => x.Name == fileField.Name) && string.IsNullOrWhiteSpace(request.Form[textField.Name]))
            {
                modelState.AddModelError(textField.Name, $"{question.Label} is required");
                return null;
            }

            modelState.SetModelValue(textField.Name, request.Form[textField.Name], request.Form[textField.Name]);
            modelState.MarkFieldValid(textField.Name);
            return request.Form[textField.Name].ToString();
        }
    }
}
