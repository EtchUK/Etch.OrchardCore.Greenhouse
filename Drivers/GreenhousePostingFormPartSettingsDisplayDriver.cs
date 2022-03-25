﻿using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.ViewModels;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Drivers
{
    public class GreenhousePostingFormPartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver
    {
        public override IDisplayResult Edit(ContentTypePartDefinition contentTypePartDefinition, IUpdateModel updater)
        {
            if (!string.Equals(nameof(GreenhousePostingFormPart), contentTypePartDefinition.PartDefinition.Name))
            {
                return null;
            }

            return Initialize<GreenhousePostingFormPartSettingsViewModel>("GreenhousePostingFormPartSettings_Edit", model =>
            {
                var settings = contentTypePartDefinition.GetSettings<GreenhousePostingFormPartSettings>();

                model.AllowedFileExtensions = string.Join(",", settings.AllowedFileExtensions ?? Array.Empty<string>());
                model.ApplicationErrorMessage = settings.ApplicationErrorMessage;
                model.ApplicationSuccessUrl = settings.ApplicationSuccessUrl;
                model.MaxFileSize = settings.MaxFileSize;
                model.ShowApplicationForm = settings.ShowApplicationForm;
                model.SubmitButtonLabel = settings.SubmitButtonLabel;
                model.ValidationErrorsMessage = settings.ValidationErrorsMessage;
            }).Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
        {
            if (!string.Equals(nameof(GreenhousePostingFormPart), contentTypePartDefinition.PartDefinition.Name))
            {
                return null;
            }

            var model = new GreenhousePostingFormPartSettingsViewModel();

            await context.Updater.TryUpdateModelAsync(model, Prefix);

            context.Builder.WithSettings(new GreenhousePostingFormPartSettings
            {
                AllowedFileExtensions = model.AllowedFileExtensions.Split(",", StringSplitOptions.RemoveEmptyEntries),
                ApplicationErrorMessage = model.ApplicationErrorMessage,
                ApplicationSuccessUrl = model.ApplicationSuccessUrl,
                MaxFileSize = model.MaxFileSize,
                ShowApplicationForm = model.ShowApplicationForm,
                SubmitButtonLabel = model.SubmitButtonLabel,
                ValidationErrorsMessage = model.ValidationErrorsMessage
            });

            return Edit(contentTypePartDefinition, context.Updater);
        }
    }
}
