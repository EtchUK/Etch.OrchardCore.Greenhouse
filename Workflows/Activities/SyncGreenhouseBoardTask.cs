using Etch.OrchardCore.Greenhouse.Services;
using Etch.OrchardCore.Greenhouse.Services.Options;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Workflows.Activities
{
    public class SyncGreenhouseBoardTask : TaskActivity
    {
        #region Constants

        private const string OutcomeDone = "Done";
        private const string OutcomeFailed = "Failed";

        #endregion Constants

        #region Dependencies

        private readonly IGreenhouseApiService _greenhouseApiService;
        private readonly IGreenhousePostingService _greenhousePostingService;
        private readonly ILogger<SyncGreenhouseBoardTask> _logger;

        private IStringLocalizer T { get; }

        #endregion Dependencies

        #region Constructor

        public SyncGreenhouseBoardTask(IGreenhouseApiService greenhouseService, IGreenhousePostingService greenhousePostingService, ILogger<SyncGreenhouseBoardTask> logger, IStringLocalizer<SyncGreenhouseBoardTask> stringLocalizer)
        {
            _greenhouseApiService = greenhouseService;
            _greenhousePostingService = greenhousePostingService;
            _logger = logger;

            T = stringLocalizer;
        }

        #endregion Constructor

        #region Overrides

        public override string Name => "Sync Greenhouse Board";

        public override LocalizedString DisplayText => T["Synchronise content items with postings retrieved from a Greenhouse job board."];

        public override LocalizedString Category => T["Greenhouse"];

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(T[OutcomeDone], T[OutcomeFailed]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                var options = new GreenhouseSyncBoardOptions
                {
                    Author = string.IsNullOrWhiteSpace(Author.Expression) ? Constants.Defaults.Author : Author.Expression,
                    BoardToken = BoardToken.Expression,
                    ContentType = string.IsNullOrWhiteSpace(ContentType.Expression) ? Constants.Defaults.ContentType : ContentType.Expression,
                    UrlPrefix = string.IsNullOrWhiteSpace(UrlPrefix.Expression) ? string.Empty : UrlPrefix.Expression
                };

                await _greenhousePostingService.SyncAsync(
                    await _greenhouseApiService.GetJobBoardAsync(options.BoardToken), 
                    new GreenhouseSyncOptions
                    {
                        Author = options.Author,
                        ContentType = options.ContentType,
                        PreventDuplicatePostingsForSameJob = false,
                        UrlPrefix = options.UrlPrefix
                    }
                );

                return Outcomes(OutcomeDone);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Outcomes(OutcomeFailed);
            }
        }

        #endregion

        #region Workflow Parameters

        public WorkflowExpression<string> Author
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> BoardToken
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> ContentType
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> UrlPrefix
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        #endregion
    }
}
