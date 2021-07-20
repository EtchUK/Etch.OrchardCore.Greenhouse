using Etch.OrchardCore.Greenhouse.Extensions;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Routing;
using OrchardCore.Entities;
using OrchardCore.Settings;
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

        #endregion Dependancies

        #region Constructor

        public ApplyController(IAutorouteEntries autorouteEntries, IContentManager contentManager, IGreenhouseApplyService greenhouseApplyService, ILogger<ApplyController> logger, ISiteService siteService)
        {
            _autorouteEntries = autorouteEntries;
            _contentManager = contentManager;
            _greenhouseApplyService = greenhouseApplyService;
            _logger = logger;
            _siteService = siteService;
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
                candidate = _greenhouseApplyService.Bind(ModelState, Request, jobPosting);
            } 
            catch (Exception ex)
            {
                TempData["ModelState"] = ModelState.Serialize();
                _logger.LogError(ex, "Error when binding/valdating Greenhouse application");
                return new RedirectResult(referer);
            }

            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState.Serialize();
                return new RedirectResult(referer);
            }

            try
            {
                var response = await _greenhouseApplyService.ApplyAsync(candidate);
                return new RedirectResult($"{Request.PathBase}{settings.DefaultSuccessUrl}?applicationId={response.Id}&contentItemId={contentItem.ContentItemId}");
            }
            catch (Exception ex)
            {
                TempData["ModelState"] = ModelState.Serialize();
                _logger.LogError(ex, "Error submitting Greenhouse application");
                return new RedirectResult(referer);
            }
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

        #endregion
    }
}
