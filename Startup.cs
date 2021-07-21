﻿using Etch.OrchardCore.Greenhouse.Display;
using Etch.OrchardCore.Greenhouse.Drivers;
using Etch.OrchardCore.Greenhouse.Filters;
using Etch.OrchardCore.Greenhouse.Indexes;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.ViewModels;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.Drivers;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Indexing;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Workflows.Helpers;
using YesSql.Indexes;

namespace Etch.OrchardCore.Greenhouse
{
    public class Startup : StartupBase
    {
        static Startup()
        {
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhouseJobPosting>();
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhouseQuestion>();
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhousePostingFormPartSettings>();
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhousePostingFormPartViewModel>();
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhousePostingPartViewModel>();
            TemplateContext.GlobalMemberAccessStrategy.Register<GreenhouseQuestionValue>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddScoped<IDisplayDriver<ISite>, GreenhouseSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            services.AddContentPart<GreenhousePostingPart>()
                .UseDisplayDriver<GreenhousePostingPartDisplayDriver>();

            services.AddContentPart<GreenhousePostingFormPart>()
                .UseDisplayDriver<GreenhousePostingFormPartDisplayDriver>();

            services.AddScoped<IContentTypePartDefinitionDisplayDriver, GreenhousePostingFormPartSettingsDisplayDriver>();

            services.AddScoped<IDataMigration, Migrations>();

            services.AddActivity<SyncGreenhousePostingsTask, SyncGreenhousePostingsTaskDisplay>();
            services.AddActivity<GreenhouseApplicationNotificationEvent, GreenhouseApplicationNotificationDisplay>();

            services.AddScoped<IGreenhouseApiService, GreenhouseApiService>();
            services.AddScoped<IGreenhouseApplyService, GreenhouseApplyService>();
            services.AddScoped<IGreenhousePostingService, GreenhousePostingService>();

            services.AddSingleton<IIndexProvider, GreenhousePostingPartIndexProvider>();
            services.AddScoped<IContentPartIndexHandler, GreenhousePostingPartIndexHandler>();

            services.AddLiquidFilter<DepartmentOptionsFilter>("greenhouse_department_options");
            services.AddLiquidFilter<LocationOptionsFilter>("greenhouse_location_options");

            services.AddScoped<IGreenhouseQuestionShapeFactory, GreenhouseQuestionShapeFactory>();
        }
    }
}
