using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Helpers;
using OrchardCore.Workflows.Models;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Workflows.Activities
{
    public class GreenhouseApplicationNotificationEvent : Activity, IEvent
    {
        #region Dependencies

        public IStringLocalizer T { get; }

        #endregion Dependencies

        #region Constructor

        public GreenhouseApplicationNotificationEvent(IStringLocalizer<GreenhouseApplicationNotificationEvent> localizer)
        {
            T = localizer;
        }

        #endregion Constructor

        #region Activity

        public override string Name => nameof(GreenhouseApplicationNotificationEvent);

        public override LocalizedString Category => T["Greenhouse"];

        public override LocalizedString DisplayText => T["Greenhouse Application Event"];

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var model = GetViewModel(workflowContext);

            SetWorkflowProperties(workflowContext, model);

            if (!model.IsSuccess)
            {
                return Outcomes("Failed");
            }

            return Outcomes("Success");
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(T["Success"], T["Failed"]);
        }

        #endregion Activity

        #region Helper Methods

        private GreenhouseApplicationNotificationEventViewModel GetViewModel(WorkflowExecutionContext workflowExecutionContext)
        {
            return workflowExecutionContext.Input.GetValue<GreenhouseApplicationNotificationEventViewModel>(nameof(GreenhouseApplicationNotificationEventViewModel)) ??
                workflowExecutionContext.Properties.GetValue<GreenhouseApplicationNotificationEventViewModel>(nameof(GreenhouseApplicationNotificationEventViewModel));
        }

        private void SetWorkflowProperties(WorkflowExecutionContext workflowContext, GreenhouseApplicationNotificationEventViewModel viewModel)
        {
            foreach (var property in viewModel.GetType().GetProperties())
            {
                workflowContext.Properties[property.Name] = property.GetValue(viewModel);
            }
        }

        #endregion
    }
}
