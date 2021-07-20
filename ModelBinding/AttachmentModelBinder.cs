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

        private readonly GreenhouseSettings _settings;

        #endregion

        #region Constructor

        public AttachmentModelBinder(GreenhouseSettings settings)
        {
            _settings = settings;
        }

        #endregion

        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            if (question.Required && !request.Form.Files.Any(x => x.Name == question.Name))
            {
                modelState.AddModelError(question.Name, $"{question.Label} is required");
                return null;
            }

            var postedFile = request.Form.Files.SingleOrDefault(x => x.Name == question.Name);

            if (postedFile == null)
            {
                modelState.MarkFieldValid(question.Name);
                return null;
            }

            if (!_settings.AllowedFileExtensions.Any(x => x.Trim().Equals(Path.GetExtension(postedFile.FileName), StringComparison.OrdinalIgnoreCase)))
            {
                modelState.AddModelError(question.Name, $"File extension is not allowed: {Path.GetExtension(postedFile.FileName)}");
                return null;
            }

            if (postedFile.Length > _settings.MaxFileSize)
            {
                modelState.AddModelError(question.Name, $"File exceeds maximum file size");
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
