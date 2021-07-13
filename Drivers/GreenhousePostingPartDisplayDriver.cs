using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.ViewModels;
using Newtonsoft.Json;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Drivers
{
    public class GreenhousePostingPartDisplayDriver : ContentPartDisplayDriver<GreenhousePostingPart>
    {
        public override IDisplayResult Display(GreenhousePostingPart part)
        {
            if (string.IsNullOrEmpty(part.PostingData))
            {
                return null;
            }

            return Initialize<GreenhousePostingPartViewModel>("GreenhousePostingPart", model =>
            {
                model.Job = JsonConvert.DeserializeObject<GreenhouseJob>(part.JobData);
                model.Posting = JsonConvert.DeserializeObject<GreenhouseJobPosting>(part.PostingData);
            }).Location("Detail", "Content:5");
        }

        public override IDisplayResult Edit(GreenhousePostingPart part)
        {
            return Initialize<GreenhousePostingPartEditViewModel>("GreenhousePostingPart_Edit", model =>
            {
                model.IgnoreSync = part.IgnoreSync;

                if (!string.IsNullOrEmpty(part.JobData))
                {
                    var job = JsonConvert.DeserializeObject<GreenhouseJob>(part.JobData);
                    model.JobData = JsonConvert.SerializeObject(job, Formatting.Indented);
                }

                if (!string.IsNullOrEmpty(part.PostingData)) {
                    var posting = JsonConvert.DeserializeObject<GreenhouseJobPosting>(part.PostingData);
                    model.PostingData = JsonConvert.SerializeObject(posting, Formatting.Indented);
                }

            }).Location("Parts:0#Greenhouse");
        }

        public override async Task<IDisplayResult> UpdateAsync(GreenhousePostingPart part, IUpdateModel updater)
        {
            var model = new GreenhousePostingPartEditViewModel();

            if (!await updater.TryUpdateModelAsync(model, Prefix))
            {
                return Edit(part);
            }

            if (!string.IsNullOrWhiteSpace(model.PostingData))
            {
                try
                {
                    var posting = JsonConvert.DeserializeObject<GreenhouseJobPosting>(model.PostingData);
                    model.PostingData = JsonConvert.SerializeObject(posting);
                }
                catch
                {
                    updater.ModelState.AddModelError(nameof(GreenhousePostingPart.PostingData), "Unable to parse data to Greenhouse job posting.");
                    return Edit(part);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.JobData))
            {
                try
                {
                    var job = JsonConvert.DeserializeObject<GreenhouseJob>(model.JobData);
                    model.JobData = JsonConvert.SerializeObject(job);
                }
                catch
                {
                    updater.ModelState.AddModelError(nameof(GreenhousePostingPart.JobData), "Unable to parse data to Greenhouse job.");
                    return Edit(part);
                }
            }

            part.IgnoreSync = model.IgnoreSync;
            part.JobData = model.JobData;
            part.PostingData = model.PostingData;

            return Edit(part);
        }
    }
}
