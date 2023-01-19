using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Entities;
using OrchardCore.ReCaptcha.Configuration;
using OrchardCore.ReCaptcha.Services;
using OrchardCore.Settings;
using OrchardCore.Workflows.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Etch.OrchardCore.Greenhouse.Constants;

namespace Etch.OrchardCore.Greenhouse.Controllers
{
    public class ApplyController : Controller
    {
        #region Dependencies

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGreenhouseApplyService _greenhouseApplyService;
        private readonly ILogger<ApplyController> _logger;
        private readonly ReCaptchaClient _reCaptchaClient;
        private readonly ISiteService _siteService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWorkflowManager _workflowManager;

        #endregion Dependancies

        #region Constructor

        public ApplyController(
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IHttpContextAccessor httpContextAccessor,
            IGreenhouseApplyService greenhouseApplyService,
            ILogger<ApplyController> logger,
            ReCaptchaClient reCaptchaClient,
            ISiteService siteService,
            IUrlHelperFactory urlHelperFactory,
            IWorkflowManager workflowManager
        )
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;
            _greenhouseApplyService = greenhouseApplyService;
            _logger = logger;
            _reCaptchaClient = reCaptchaClient;
            _siteService = siteService;
            _urlHelperFactory = urlHelperFactory;
            _workflowManager = workflowManager;
        }

        #endregion

        [HttpPost]
        [Route("greenhouse/apply/{contentItemId}")]
        public async Task<ActionResult> Index(string contentItemId)
        {
            if (string.IsNullOrWhiteSpace(contentItemId))
            {
                return NotFound();
            }

            var contentItem = await GetContentItemAsync(contentItemId);

            if (contentItem == null)
            {
                return new BadRequestResult();
            }

            var jobPosting = contentItem.As<GreenhousePostingPart>().GetJobPostingData();
            var formPartSettings = GetFormPartSettings(contentItem);
            var returnUrl = await GetContentItemUrlAsync(contentItem);

            GreenhouseApplication application;

            try
            {
                application = _greenhouseApplyService.Bind(ModelState, Request, jobPosting, formPartSettings);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validiting Greenhouse application");
                return await HandleErrorAsync(jobPosting, ex.Message, GreenhouseApplicationPhases.Validation, formPartSettings.ApplicationErrorMessage, returnUrl);
            }

            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult($"{returnUrl}");
            }

            if (formPartSettings.UseReCaptcha && !await ValidateRecpatchaAsync())
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult($"{returnUrl}");
            }

            GreenhouseApplicationResponse response;

            try
            {
                response = await _greenhouseApplyService.ApplyAsync(jobPosting.Id, application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting Greenhouse application");
                return await HandleErrorAsync(jobPosting, ex.Message, GreenhouseApplicationPhases.Apply, formPartSettings.ApplicationErrorMessage, returnUrl, application);
            }

            if (!response.Success)
            {
                return await HandleErrorAsync(jobPosting, response.Error, GreenhouseApplicationPhases.Apply, formPartSettings.ApplicationErrorMessage, returnUrl, application);
            }

            await TriggerNotificationEventAsync(jobPosting, new GreenhouseApplicationNotificationEventViewModel
            {
                Application = application,
                Error = response.Error,
                IsSuccess = response.Success,
                Response = response
            });

            return new RedirectResult($"{Request.PathBase}{formPartSettings.ApplicationSuccessUrl}?contentItemId={contentItem.ContentItemId}");
        }

        private async Task<bool> ValidateRecpatchaAsync()
        {
            var reCaptchaResponse = _httpContextAccessor.HttpContext?.Request.Headers["g-recaptcha-response"];

            if (string.IsNullOrEmpty(reCaptchaResponse) && (_httpContextAccessor.HttpContext?.Request.HasFormContentType ?? false))
            {
                reCaptchaResponse = _httpContextAccessor.HttpContext.Request.Form["g-recaptcha-response"].ToString();
            }

            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var recaptchaSettings = siteSettings.As<ReCaptchaSettings>();

            return !string.IsNullOrEmpty(reCaptchaResponse) && await _reCaptchaClient.VerifyAsync(reCaptchaResponse, recaptchaSettings.SecretKey);
        }

        #region Helper Methods

        private async Task<ContentItem> GetContentItemAsync(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId);

            if (contentItem == null || !contentItem.Has<GreenhousePostingPart>())
            {
                return null;
            }

            return contentItem;
        }

        private async Task<string> GetContentItemUrlAsync(ContentItem contentItem)
        {
            var contentItemMetadata = await _contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
            var routeValues = contentItemMetadata.DisplayRouteValues;
            return $"{_urlHelperFactory.GetUrlHelper(ControllerContext).RouteUrl(routeValues)}#{Constants.AnchorUrl}";
        }

        private GreenhousePostingFormPartSettings GetFormPartSettings(ContentItem contentItem)
        {
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);
            var partDefinition = typeDefinition.Parts.FirstOrDefault(x => x.Name == nameof(GreenhousePostingFormPart));

            if (partDefinition == null)
            {
                return null;
            }

            return partDefinition.GetSettings<GreenhousePostingFormPartSettings>();
        }

        private async Task<ActionResult> HandleErrorAsync(GreenhouseJobPosting posting, string error, string phase, string userFeedback, string returnUrl, GreenhouseApplication application = null)
        {
            await TriggerNotificationEventAsync(posting, new GreenhouseApplicationNotificationEventViewModel
            {
                Application = application,
                Error = error,
                IsSuccess = false,
                Phase = phase
            });

            TempData[TempDataKeys.ApplicationError] = userFeedback;
            TempData["ModelState"] = ModelState.Serialize();
            return new RedirectResult($"{returnUrl}#apply");
        }

        private async Task TriggerNotificationEventAsync(GreenhouseJobPosting posting, GreenhouseApplicationNotificationEventViewModel viewModel)
        {
            await _workflowManager.TriggerEventAsync(
                nameof(GreenhouseApplicationNotificationEvent),
                input: new
                {
                    GreenhouseApplicationNotificationEventViewModel = viewModel
                },
                correlationId: posting.Id.ToString()
            );
        }

        #endregion
    }
}
