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
        public override IDisplayResult Edit(GreenhousePostingPart part)
        {
            return Initialize<GreenhousePostingPartEditViewModel>("GreenhousePostingPart_Edit", model =>
            {
                if (!string.IsNullOrEmpty(part.Data)) {
                    var posting = JsonConvert.DeserializeObject<GreenhouseJobPostingDto>(part.Data);
                    model.Data = JsonConvert.SerializeObject(posting, Formatting.Indented);
                }

                model.IgnoreSync = part.IgnoreSync;
            }).Location("Parts:0#Greenhouse");
        }

        public override async Task<IDisplayResult> UpdateAsync(GreenhousePostingPart part, IUpdateModel updater)
        {
            var model = new GreenhousePostingPartEditViewModel();

            if (!await updater.TryUpdateModelAsync(model, Prefix))
            {
                return Edit(part);
            }

            try
            {
                var posting = JsonConvert.DeserializeObject<GreenhouseJobPostingDto>(model.Data);
                model.Data = JsonConvert.SerializeObject(posting);
            } 
            catch
            {
                updater.ModelState.AddModelError(nameof(GreenhousePostingPart.Data), "Unable to parse data to Greenhouse job posting");
                return Edit(part);
            }

            part.Data = model.Data;
            part.IgnoreSync = model.IgnoreSync;

            return Edit(part);
        }
    }
}
