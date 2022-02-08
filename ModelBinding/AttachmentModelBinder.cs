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
            var field = question.Fields.FirstOrDefault();

            if (question.Required.HasValue && question.Required.Value && !request.Form.Files.Any(x => x.Name == field.Name))
            {
                modelState.AddModelError(field.Name, $"{question.Label} is required");
                return null;
            }

            var postedFile = request.Form.Files.SingleOrDefault(x => x.Name == field.Name);

            if (postedFile == null)
            {
                modelState.MarkFieldValid(field.Name);
                return null;
            }

            if (!_settings.AllowedFileExtensions.Any(x => x.Trim().Equals(Path.GetExtension(postedFile.FileName), StringComparison.OrdinalIgnoreCase)))
            {
                modelState.AddModelError(field.Name, $"File extension is not allowed: {Path.GetExtension(postedFile.FileName)}");
                return null;
            }

            if (postedFile.Length > _settings.MaxFileSize)
            {
                modelState.AddModelError(field.Name, $"File exceeds maximum file size");
                return null;
            }

            modelState.MarkFieldValid(field.Name);

            if (!request.Form.Files.Any(x => x.Name == field.Name))
            {
                return null;
            }

            return new GreenhouseAttachment {
                Content = request.Form.Files[field.Name].ToBase64(),
                ContentType = request.Form.Files[field.Name].ContentType,
                Filename = request.Form.Files[field.Name].FileName,
                Type = field.Name
            };
        }
    }
}
