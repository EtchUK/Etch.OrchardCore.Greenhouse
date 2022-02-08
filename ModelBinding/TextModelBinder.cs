﻿using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class TextModelBinder : IGreenhouseCandidateModelBinder
    {
        public object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question)
        {
            var field = question.Fields.FirstOrDefault();

            modelState.SetModelValue(field.Name, request.Form[field.Name], request.Form[field.Name]);

            if (question.Required.HasValue && question.Required.Value && string.IsNullOrWhiteSpace(request.Form[field.Name]))
            {
                modelState.AddModelError(field.Name, $"{question.Label} is required");
                return request.Form[field.Name].ToString();
            }

            modelState.MarkFieldValid(field.Name);
            return request.Form[field.Name].ToString();
        }
    }
}
