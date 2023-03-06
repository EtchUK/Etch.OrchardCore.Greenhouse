using Etch.OrchardCore.Greenhouse.Display;
using Etch.OrchardCore.Greenhouse.Drivers;
using Etch.OrchardCore.Greenhouse.Filters;
using Etch.OrchardCore.Greenhouse.Indexes;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Resources;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.ViewModels;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.Drivers;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Indexing;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.ResourceManagement;
using OrchardCore.Settings;
using OrchardCore.Workflows.Helpers;
using YesSql.Indexes;

namespace Etch.OrchardCore.Greenhouse
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var T = services.BuildServiceProvider().GetService<IStringLocalizerFactory>().Create("ModelBindingMessages", "WebUI");

                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => T["The value is invalid."]);
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => T["The field {0} must be a number."]);
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => T["A value for the '{0}' property was not provided.", x]);
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => T["The value is not valid for {1}.", x, y]);
                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => T["The supplied value is invalid for {0}.", x]);
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => T["Null value is invalid.", x]);
            });

            services.AddScoped<IDisplayDriver<ISite>, GreenhouseSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            services.AddContentPart<GreenhousePostingPart>()
                .UseDisplayDriver<GreenhousePostingPartDisplayDriver>();

            services.AddContentPart<GreenhousePostingFormPart>()
                .UseDisplayDriver<GreenhousePostingFormPartDisplayDriver>();

            services.AddScoped<IContentTypePartDefinitionDisplayDriver, GreenhousePostingFormPartSettingsDisplayDriver>();

            services.AddScoped<IDataMigration, Migrations>();

            services.AddActivity<SyncGreenhouseBoardTask, SyncGreenhouseBoardTaskDisplay>();
            services.AddActivity<GreenhouseApplicationNotificationEvent, GreenhouseApplicationNotificationDisplay>();

            services.AddScoped<IGreenhouseApiService, GreenhouseApiService>();
            services.AddScoped<IGreenhouseApplyService, GreenhouseApplyService>();
            services.AddScoped<IGreenhousePostingService, GreenhousePostingService>();

            services.AddSingleton<IIndexProvider, GreenhousePostingPartIndexProvider>();
            services.AddScoped<IContentPartIndexHandler, GreenhousePostingPartIndexHandler>();

            services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();

            services.Configure<TemplateOptions>(o =>
            {
                o.MemberAccessStrategy.Register<GreenhouseField>();
                o.MemberAccessStrategy.Register<GreenhouseJobPosting>();
                o.MemberAccessStrategy.Register<GreenhouseQuestion>();
                o.MemberAccessStrategy.Register<GreenhousePostingFormPartSettings>();
                o.MemberAccessStrategy.Register<GreenhousePostingFormPartViewModel>();
                o.MemberAccessStrategy.Register<GreenhousePostingPartViewModel>();
                o.MemberAccessStrategy.Register<GreenhouseQuestionDisplayContext>();
                o.MemberAccessStrategy.Register<GreenhouseQuestionValue>();
            });

            services.AddLiquidFilter<DepartmentOptionsFilter>("greenhouse_department_options");
            services.AddLiquidFilter<DisplayMetadataFilter>("greenhouse_display_meta");
            services.AddLiquidFilter<LocationOptionsFilter>("greenhouse_location_options");
            services.AddLiquidFilter<UniqueDepartmentsFilter>("greenhouse_unique_departments");
            services.AddLiquidFilter<MetadataOptionsFilter>("greenhouse_metadata_options");


            services.AddScoped<IGreenhouseQuestionShapeFactory, GreenhouseQuestionShapeFactory>();
        }
    }
}
