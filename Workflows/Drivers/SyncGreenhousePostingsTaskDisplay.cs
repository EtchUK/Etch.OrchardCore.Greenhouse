using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace Etch.OrchardCore.Greenhouse.Workflows.Drivers
{
    public class SyncGreenhousePostingsTaskDisplay : ActivityDisplayDriver<SyncGreenhousePostingsTask, SyncGreenhousePostingsTaskViewModel>
    {
        protected override void EditActivity(SyncGreenhousePostingsTask activity, SyncGreenhousePostingsTaskViewModel model)
        {
            model.Author = activity.Author.Expression;
            model.ContentType = activity.ContentType.Expression;
            model.ExternalOnly = bool.Parse(activity.ExternalOnly.Expression ?? "False");
            model.Locations = activity.Locations.Expression ?? "[]";
            model.PreventDuplicatePostingsForSameJob = bool.Parse(activity.PreventDuplicatePostingsForSameJob.Expression ?? "False");
        }

        protected override void UpdateActivity(SyncGreenhousePostingsTaskViewModel model, SyncGreenhousePostingsTask activity)
        {
            activity.Author = new WorkflowExpression<string>(model.Author);
            activity.ContentType = new WorkflowExpression<string>(model.ContentType);
            activity.ExternalOnly = new WorkflowExpression<bool>(model.ExternalOnly.ToString());
            activity.Locations = new WorkflowExpression<string>(model.Locations);
            activity.PreventDuplicatePostingsForSameJob = new WorkflowExpression<bool>(model.PreventDuplicatePostingsForSameJob.ToString());
        }
    }
}
