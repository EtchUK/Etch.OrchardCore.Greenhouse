using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Drivers
{
    public class GreenhouseSettingsDisplayDriver : SectionDisplayDriver<ISite, GreenhouseSettings>
    {
        #region Dependencies

        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public GreenhouseSettingsDisplayDriver(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Overrides

        public override async Task<IDisplayResult> EditAsync(GreenhouseSettings section, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageGreenhouseSettings))
            {
                return null;
            }

            return Initialize<GreenhouseSettingsViewModel>("GreenhouseSettings_Edit", model =>
            {
                model.ApiHostname = section.ApiHostname;
                model.ApiKey = section.ApiKey;
            }).Location("Content:3").OnGroup(Constants.GroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(GreenhouseSettings section, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageGreenhouseSettings))
            {
                return null;
            }

            if (context.GroupId == Constants.GroupId)
            {
                var model = new GreenhouseSettingsViewModel();

                if (await context.Updater.TryUpdateModelAsync(model, Prefix))
                {
                    section.ApiHostname = string.IsNullOrWhiteSpace(model.ApiHostname) ? Constants.Defaults.ApiHostname : model.ApiHostname.TrimEnd('/');
                    section.ApiKey = model.ApiKey;
                }
            }

            return await EditAsync(section, context);
        }

        #endregion
    }
}
