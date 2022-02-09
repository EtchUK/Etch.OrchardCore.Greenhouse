using Etch.OrchardCore.Greenhouse.Workflows.Activities;
using Etch.OrchardCore.Greenhouse.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace Etch.OrchardCore.Greenhouse.Workflows.Drivers
{
    public class SyncGreenhouseBoardTaskDisplay : ActivityDisplayDriver<SyncGreenhouseBoardTask, SyncGreenhouseBoardTaskViewModel>
    {
        protected override void EditActivity(SyncGreenhouseBoardTask activity, SyncGreenhouseBoardTaskViewModel model)
        {
            model.Author = activity.Author.Expression;
            model.ContentType = activity.ContentType.Expression;
            model.UrlPrefix = activity.UrlPrefix.Expression;
        }

        protected override void UpdateActivity(SyncGreenhouseBoardTaskViewModel model, SyncGreenhouseBoardTask activity)
        {
            activity.Author = new WorkflowExpression<string>(model.Author);
            activity.ContentType = new WorkflowExpression<string>(model.ContentType);
            activity.UrlPrefix = new WorkflowExpression<string>(model.UrlPrefix);
        }
    }
}
