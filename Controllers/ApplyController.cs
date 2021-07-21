using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Routing;
using OrchardCore.Entities;
using OrchardCore.Settings;
using OrchardCore.Workflows.Services;
using System;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Controllers
{
    public class ApplyController : Controller
    {
        #region Dependencies

        private readonly IAutorouteEntries _autorouteEntries;
        private readonly IContentManager _contentManager;
        private readonly IGreenhouseApplyService _greenhouseApplyService;
        private readonly ILogger<ApplyController> _logger;
        private readonly ISiteService _siteService;
        private readonly IWorkflowManager _workflowManager;

        #endregion Dependancies

        #region Constructor

        public ApplyController(IAutorouteEntries autorouteEntries, IContentManager contentManager, IGreenhouseApplyService greenhouseApplyService, ILogger<ApplyController> logger, ISiteService siteService, IWorkflowManager workflowManager)
        {
            _autorouteEntries = autorouteEntries;
            _contentManager = contentManager;
            _greenhouseApplyService = greenhouseApplyService;
            _logger = logger;
            _siteService = siteService;
            _workflowManager = workflowManager;
        }

        #endregion

        [HttpPost]
        [Route("greenhouse/apply")]
        public async Task<ActionResult> Index()
        {
            var referer = Request.Headers["Referer"].ToString();
            var settings = (await _siteService.GetSiteSettingsAsync()).As<GreenhouseSettings>();

            _autorouteEntries.TryGetEntryByPath(GetReferrerRoute(), out var entry);

            if (entry == null)
            {
                return new BadRequestResult();
            }

            var contentItem = await _contentManager.GetAsync(entry.ContentItemId);

            if (contentItem == null || !contentItem.Has<GreenhousePostingPart>())
            {
                return new BadRequestResult();
            }

            var jobPosting = contentItem.As<GreenhousePostingPart>().GetJobPostingData();
            GreenhouseCandidate candidate;

            try
            {
                candidate = await _greenhouseApplyService.BindAsync(ModelState, Request, jobPosting);
            } 
            catch (Exception ex)
            {
                await TriggerNotificationEventAsync(jobPosting, new GreenhouseApplicationNotificationEventViewModel
                {
                    Error = ex.Message,
                    IsSuccess = false,
                    Phase = Constants.GreenhouseApplicationPhases.Validation
                });

                TempData["ModelState"] = ModelState.Serialize();
                _logger.LogError(ex, "Error when binding/valdating Greenhouse application");
                return new RedirectResult(referer);
            }

            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult(referer);
            }

            GreenhouseCandidateResponse response;

            try
            {
                response = await _greenhouseApplyService.ApplyAsync(candidate);
            }
            catch (Exception ex)
            {
                await TriggerNotificationEventAsync(jobPosting, new GreenhouseApplicationNotificationEventViewModel
                {
                    Candidate = candidate,
                    Error = ex.Message,
                    IsSuccess = false,
                    Phase = Constants.GreenhouseApplicationPhases.Apply
                });

                TempData["ModelState"] = ModelState.Serialize();
                _logger.LogError(ex, "Error submitting Greenhouse application");
                return new RedirectResult(referer);
            }

            await TriggerNotificationEventAsync(jobPosting, new GreenhouseApplicationNotificationEventViewModel
            {
                Candidate = candidate,
                IsSuccess = true,
                Response = response
            });

            return new RedirectResult($"{Request.PathBase}{settings.DefaultSuccessUrl}?applicationId={response.Id}&contentItemId={contentItem.ContentItemId}");
        }

        #region Helper Methods

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
