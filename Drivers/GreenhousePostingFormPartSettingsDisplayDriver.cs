using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.ViewModels;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Drivers
{
    public class GreenhousePostingFormPartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver
    {
        private readonly IShellFeaturesManager _shellFeaturesManager;

        public GreenhousePostingFormPartSettingsDisplayDriver(IShellFeaturesManager shellFeaturesManager)
        {
            _shellFeaturesManager = shellFeaturesManager;
        }

        public override async Task<IDisplayResult> EditAsync(ContentTypePartDefinition contentTypePartDefinition, IUpdateModel updater)
        {
            if (!string.Equals(nameof(GreenhousePostingFormPart), contentTypePartDefinition.PartDefinition.Name))
            {
                return null;
            }

            var enabledFeatures = await _shellFeaturesManager.GetEnabledExtensionsAsync();

            return Initialize<GreenhousePostingFormPartSettingsViewModel>("GreenhousePostingFormPartSettings_Edit", model =>
            {
                var settings = contentTypePartDefinition.GetSettings<GreenhousePostingFormPartSettings>();

                model.AllowedFileExtensions = string.Join(",", settings.AllowedFileExtensions ?? Array.Empty<string>());
                model.ApplicationErrorMessage = settings.ApplicationErrorMessage;
                model.ApplicationSuccessUrl = settings.ApplicationSuccessUrl;
                model.HasReCaptchaEnabled = enabledFeatures.Any(x => x.Id == "OrchardCore.ReCaptcha");
                model.MaxFileSize = settings.MaxFileSize;
                model.ShowApplicationForm = settings.ShowApplicationForm;
                model.SubmitButtonLabel = settings.SubmitButtonLabel;
                model.UseReCaptcha = settings.UseReCaptcha;
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
                UseReCaptcha = model.UseReCaptcha,
                ValidationErrorsMessage = model.ValidationErrorsMessage
            });

            return await EditAsync(contentTypePartDefinition, context.Updater);
        }
    }
}
