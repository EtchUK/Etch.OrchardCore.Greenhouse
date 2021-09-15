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

            GreenhouseCandidate candidate;

            try
            {
                candidate = _greenhouseApplyService.Bind(ModelState, Request, jobPosting, formPartSettings);
            } 
            catch (Exception ex)
            {
                return await HandleErrorAsync(jobPosting, ex, Constants.GreenhouseApplicationPhases.Validation);
            }

            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult(Request.Headers["Referer"].ToString());
            }

            GreenhouseCandidateResponse response;

            try
            {
                response = await _greenhouseApplyService.ApplyAsync(candidate);
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync(jobPosting, ex, Constants.GreenhouseApplicationPhases.Apply, candidate);
            }

            await TriggerNotificationEventAsync(jobPosting, new GreenhouseApplicationNotificationEventViewModel
            {
                Candidate = candidate,
                IsSuccess = true,
                Response = response
            });

            return new RedirectResult($"{Request.PathBase}{formPartSettings.ApplicationSuccessUrl}?applicationId={response.Id}&contentItemId={contentItem.ContentItemId}");
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

        private async Task<ActionResult> HandleErrorAsync(GreenhouseJobPosting posting, Exception ex, string phase, GreenhouseCandidate candidate = null)
        {
            var referer = Request.Headers["Referer"].ToString();

            await TriggerNotificationEventAsync(posting, new GreenhouseApplicationNotificationEventViewModel
            {
                Candidate = candidate,
                Error = ex.Message,
                IsSuccess = false,
                Phase = phase
            });

            TempData["ModelState"] = ModelState.Serialize();
            _logger.LogError(ex, "Error submitting Greenhouse application");
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
