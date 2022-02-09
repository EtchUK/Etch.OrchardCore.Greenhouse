using Etch.OrchardCore.Greenhouse.ModelBinding;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Services
{
    public class GreenhouseApplyService : IGreenhouseApplyService
    {
        #region Dependencies

        private readonly IGreenhouseApiService _greenhouseApiSerice;

        #endregion

        #region Constructor

        public GreenhouseApplyService(IGreenhouseApiService greenhouseApiSerice)
        {
            _greenhouseApiSerice = greenhouseApiSerice;
        }

        #endregion

        public async Task<GreenhouseApplicationResponse> ApplyAsync(long jobId, GreenhouseApplication application)
        {
            return await _greenhouseApiSerice.CreateApplicationAsync(jobId, application);
        }

        public GreenhouseApplication Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings)
        {
            var binders = new Dictionary<string, IGreenhouseCandidateModelBinder>
            {
                { Constants.GreenhouseFieldTypes.Attachment, new AttachmentModelBinder(settings) },
                { Constants.GreenhouseFieldTypes.LongText, new TextModelBinder() },
                { Constants.GreenhouseFieldTypes.MultiSelect, new MultiSelectValueModelBinder() },
                { Constants.GreenhouseFieldTypes.ShortText, new TextModelBinder() },
                { Constants.GreenhouseFieldTypes.SingleSelect, new SelectedValueModelBinder() }
            };

            var application = new GreenhouseApplication
            {
                Cover = (GreenhouseAttachment)binders[Constants.GreenhouseFieldTypes.Attachment].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Cover)),
                CoverText = (string)new AttachmentTextModelBinder().Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.Any(x => x.Name == Constants.GreenhouseFieldNames.CoverText))),
                Email = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Email)),
                FirstName = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Firstname)),
                LastName = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Lastname)),
                Location = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Location)),
                Phone = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Phone)),
                Resume = (GreenhouseAttachment)binders[Constants.GreenhouseFieldTypes.Attachment].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Resume)),
                ResumeText = (string)new AttachmentTextModelBinder().Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.Any(x => x.Name == Constants.GreenhouseFieldNames.ResumeText)))
            };
            
            foreach (var question in posting.Questions.Where(x => !Constants.GreenhouseFieldNames.FixedFields.Contains(x.Fields.FirstOrDefault()?.Name)))
            {
                var field = question.Fields.FirstOrDefault();

                if (field == null)
                {
                    continue;
                }

                application.Questions.Add(new GreenhouseCustomField
                {
                    Name = question.Fields.FirstOrDefault()?.Name,
                    Value = binders[field.Type].Bind(modelState, request, question)
                });
            }

            return application;
        }
    }

    public interface IGreenhouseApplyService
    {
        Task<GreenhouseApplicationResponse> ApplyAsync(long jobId, GreenhouseApplication application);
        GreenhouseApplication Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings);
    }
}
