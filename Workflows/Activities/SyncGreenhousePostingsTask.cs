using Etch.OrchardCore.Greenhouse.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Workflows.Activities
{
    public class SyncGreenhousePostingsTask : TaskActivity
    {
        #region Constants

        private const string OutcomeDone = "Done";
        private const string OutcomeFailed = "Failed";

        #endregion Constants

        #region Dependencies

        private readonly IGreenhouseApiService _greenhouseApiService;
        private readonly IGreenhousePostingService _greenhousePostingService;
        private readonly ILogger<SyncGreenhousePostingsTask> _logger;

        private IStringLocalizer T { get; }

        #endregion Dependencies

        #region Constructor

        public SyncGreenhousePostingsTask(IGreenhouseApiService greenhouseService, IGreenhousePostingService greenhousePostingService, ILogger<SyncGreenhousePostingsTask> logger, IStringLocalizer<SyncGreenhousePostingsTask> stringLocalizer)
        {
            _greenhouseApiService = greenhouseService;
            _greenhousePostingService = greenhousePostingService;
            _logger = logger;

            T = stringLocalizer;
        }

        #endregion Constructor

        #region Overrides

        public override string Name => "Sync Greenhouse Postings";

        public override LocalizedString DisplayText => T["Synchronise content items with postings retrieved from Greenhouse"];

        public override LocalizedString Category => T["Content"];

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(T[OutcomeDone], T[OutcomeFailed]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                await _greenhousePostingService.SyncAsync((await _greenhouseApiService.GetJobPostingsAsync(await _greenhousePostingService.GetLatestUpdatedAtAsync())).ToList());

                return Outcomes(OutcomeDone);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Outcomes(OutcomeFailed);
            }
        }

        #endregion
    }
}
