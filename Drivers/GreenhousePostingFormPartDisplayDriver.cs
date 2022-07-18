using Etch.OrchardCore.Greenhouse.Display;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.ViewModels;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Entities;
using OrchardCore.ReCaptcha.Configuration;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Drivers
{
    public class GreenhousePostingFormPartDisplayDriver : ContentPartDisplayDriver<GreenhousePostingFormPart>
    {
        #region Dependencies

        private readonly IGreenhouseQuestionShapeFactory _greenhouseQuestionShapeFactory;
        private readonly ISiteService _siteService;

        #endregion

        #region Constructor

        public GreenhousePostingFormPartDisplayDriver(
            IGreenhouseQuestionShapeFactory greenhouseQuestionShapeFactory,
            ISiteService siteService
            )
        {
            _greenhouseQuestionShapeFactory = greenhouseQuestionShapeFactory;
            _siteService = siteService;
        }

        #endregion

        #region Overrides

        public override async Task<IDisplayResult> DisplayAsync(GreenhousePostingFormPart part, BuildPartDisplayContext context)
        {
            if (context.DisplayType != "Detail")
            {
                return null;
            }

            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var recaptchaSettings = siteSettings.As<ReCaptchaSettings>();

            var settings = context.TypePartDefinition.GetSettings<GreenhousePostingFormPartSettings>();
            var postingPart = part.ContentItem.As<GreenhousePostingPart>();

            if (postingPart == null || string.IsNullOrEmpty(postingPart.PostingData))
            {
                return null;
            }

            var posting = JsonConvert.DeserializeObject<GreenhouseJobPosting>(postingPart.PostingData);
            IList<dynamic> questions = Array.Empty<dynamic>();

            if (part.ShowApplicationForm)
            {
                questions = await _greenhouseQuestionShapeFactory.CreateAsync(posting, settings);
            }

            return Initialize<GreenhousePostingFormPartViewModel>("GreenhousePostingFormPart", model =>
            {
                model.ContentItem = part.ContentItem;
                model.Questions = questions;
                model.Posting = posting;
                model.PostingPart = postingPart;
                model.ReCaptchaSettings = recaptchaSettings;
                model.Settings = settings;
                model.ShowApplicationForm = settings.ShowApplicationForm && part.ShowApplicationForm;
                model.UseReCaptcha = settings.UseReCaptcha && recaptchaSettings != null && recaptchaSettings.IsValid();
            }).Location("Detail", "Content:10");
        }

        public override IDisplayResult Edit(GreenhousePostingFormPart part)
        {
            return Initialize<GreenhousePostingFormPartEditViewModel>("GreenhousePostingFormPart_Edit", model =>
            {
                model.ShowApplicationForm = part.ShowApplicationForm;
            }).Location("Parts:0#Application Form");
        }

        public override async Task<IDisplayResult> UpdateAsync(GreenhousePostingFormPart part, IUpdateModel updater)
        {
            var model = new GreenhousePostingFormPartEditViewModel();

            if (!await updater.TryUpdateModelAsync(model, Prefix))
            {
                return Edit(part);
            }

            part.ShowApplicationForm = model.ShowApplicationForm;

            return Edit(part);
        }

        #endregion
    }
}
