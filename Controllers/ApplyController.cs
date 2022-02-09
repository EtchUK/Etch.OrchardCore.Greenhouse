using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Routing;
using OrchardCore.Entities;
using OrchardCore.Workflows.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Etch.OrchardCore.Greenhouse.Constants;

namespace Etch.OrchardCore.Greenhouse.Controllers
{
    public class ApplyController : Controller
    {
        #region Dependencies

        private readonly IAutorouteEntries _autorouteEntries;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IGreenhouseApplyService _greenhouseApplyService;
        private readonly ILogger<ApplyController> _logger;
        private readonly IWorkflowManager _workflowManager;

        #endregion Dependancies

        #region Constructor

        public ApplyController(IAutorouteEntries autorouteEntries, IContentDefinitionManager contentDefinitionManager, IContentManager contentManager, IGreenhouseApplyService greenhouseApplyService, ILogger<ApplyController> logger, IWorkflowManager workflowManager)
        {
            _autorouteEntries = autorouteEntries;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _greenhouseApplyService = greenhouseApplyService;
            _logger = logger;
            _workflowManager = workflowManager;
        }

        #endregion

        [HttpPost]
        [Route("greenhouse/apply")]
        public async Task<ActionResult> Index()
        {
            var contentItem = await GetContentItemAsync();

            if (contentItem == null)
            {
                return new BadRequestResult();
            }

            var jobPosting = contentItem.As<GreenhousePostingPart>().GetJobPostingData();
            var formPartSettings = GetFormPartSettings(contentItem);

            GreenhouseApplication application;

            try
            {
                application = _greenhouseApplyService.Bind(ModelState, Request, jobPosting, formPartSettings);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validiting Greenhouse application");
                return await HandleErrorAsync(jobPosting, ex.Message, Constants.GreenhouseApplicationPhases.Validation, formPartSettings.ApplicationErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult(Request.Headers["Referer"].ToString());
            }

            GreenhouseApplicationResponse response;

            try
            {
                response = await _greenhouseApplyService.ApplyAsync(jobPosting.Id, application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting Greenhouse application");
                return await HandleErrorAsync(jobPosting, ex.Message, Constants.GreenhouseApplicationPhases.Apply, formPartSettings.ApplicationErrorMessage, application);
            }

            if (!response.Success)
            {
                return await HandleErrorAsync(jobPosting, response.Error, Constants.GreenhouseApplicationPhases.Apply, formPartSettings.ApplicationErrorMessage, application);
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

        #region Helper Methods

        private async Task<ContentItem> GetContentItemAsync()
        {
            (var result, var entry) = await _autorouteEntries.TryGetEntryByPathAsync(GetReferrerRoute());

            if (!result)
            {
                return null;
            }

            var contentItem = await _contentManager.GetAsync(entry.ContentItemId);

            if (contentItem == null || !contentItem.Has<GreenhousePostingPart>())
            {
                return null;
            }

            return contentItem;
        }

        private string GetReferrerRoute()
        {
            var referer = Request.Headers["Referer"].ToString();
            referer = referer.Replace($"{Request.Scheme}://", "");
            referer = referer.Replace(Request.Host.ToString(), "");

            if (!string.IsNullOrWhiteSpace(Request.PathBase))
            {
                referer = referer.Replace(Request.PathBase, "");
            }

            return referer;
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

        private async Task<ActionResult> HandleErrorAsync(GreenhouseJobPosting posting, string error, string phase, string userFeedback, GreenhouseApplication application = null)
        {
            var referer = Request.Headers["Referer"].ToString();

            if (!referer.EndsWith("#apply"))
            {
                referer += "#apply";
            }

            await TriggerNotificationEventAsync(posting, new GreenhouseApplicationNotificationEventViewModel
            {
                Application = application,
                Error = error,
                IsSuccess = false,
                Phase = phase
            });

            TempData[TempDataKeys.ApplicationError] = userFeedback;
            TempData["ModelState"] = ModelState.Serialize();
            return new RedirectResult(referer);
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
