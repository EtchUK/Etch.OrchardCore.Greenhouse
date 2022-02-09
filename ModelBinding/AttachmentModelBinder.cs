using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class AttachmentModelBinder : IGreenhouseCandidateModelBinder
    {
        #region Dependencies

        private readonly GreenhousePostingFormPartSettings _settings;

        #endregion

        #region Constructor

        public AttachmentModelBinder(GreenhousePostingFormPartSettings settings)
        {
            _settings = settings;
        }

        #endregion

        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            if (question == null)
            {
                return null;
            }

            var fileField = question.Fields.FirstOrDefault(x => x.Type == Constants.GreenhouseFieldTypes.Attachment);
            var textField = question.Fields.FirstOrDefault(x => x.Type == Constants.GreenhouseFieldTypes.LongText);

            if (question.Required.HasValue && question.Required.Value && !request.Form.Files.Any(x => x.Name == fileField.Name) && string.IsNullOrWhiteSpace(request.Form[textField.Name]))
            {
                modelState.AddModelError(fileField.Name, $"{question.Label} is required");
                return null;
            }

            var postedFile = request.Form.Files.SingleOrDefault(x => x.Name == fileField.Name);

            if (postedFile == null)
            {
                modelState.MarkFieldValid(fileField.Name);
                return null;
            }

            if (!_settings.AllowedFileExtensions.Any(x => x.Trim().Equals(Path.GetExtension(postedFile.FileName), StringComparison.OrdinalIgnoreCase)))
            {
                modelState.AddModelError(fileField.Name, $"File extension is not allowed: {Path.GetExtension(postedFile.FileName)}");
                return null;
            }

            if (postedFile.Length > _settings.MaxFileSize)
            {
                modelState.AddModelError(fileField.Name, $"File exceeds maximum file size");
                return null;
            }

            modelState.MarkFieldValid(fileField.Name);

            if (!request.Form.Files.Any(x => x.Name == fileField.Name))
            {
                return null;
            }

            return new GreenhouseAttachment
            {
                Content = request.Form.Files[fileField.Name].ToBase64(),
                ContentType = request.Form.Files[fileField.Name].ContentType,
                Filename = request.Form.Files[fileField.Name].FileName,
                Type = fileField.Name
            };
        }
    }
}
