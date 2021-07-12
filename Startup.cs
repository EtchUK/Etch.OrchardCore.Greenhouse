using Etch.OrchardCore.Greenhouse.Drivers;
using Etch.OrchardCore.Greenhouse.Indexes;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.Drivers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Workflows.Helpers;
using YesSql.Indexes;

namespace Etch.OrchardCore.Greenhouse
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDisplayDriver<ISite>, GreenhouseSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            services.AddContentPart<GreenhousePostingPart>()
                .UseDisplayDriver<GreenhousePostingPartDisplayDriver>();

            services.AddScoped<IDataMigration, Migrations>();

            services.AddActivity<SyncGreenhousePostingsTask, SyncGreenhousePostingsTaskDisplay>();

            services.AddScoped<IGreenhousePostingService, GreenhousePostingService>();
            services.AddScoped<IGreenhouseApiService, GreenhouseApiService>();

            services.AddSingleton<IIndexProvider, GreenhousePostingPartIndexProvider>();
        }
    }
}
