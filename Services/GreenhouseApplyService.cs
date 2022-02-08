﻿using Etch.OrchardCore.Greenhouse.ModelBinding;
using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
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

        public async Task<GreenhouseCandidateResponse> ApplyAsync(GreenhouseCandidate candidate)
        {
            // create candidate
            var createdCandidate = await _greenhouseApiSerice.CreateCandidateAsync(candidate);

            // add attachments
            await _greenhouseApiSerice.AddAttachmentsAsync(createdCandidate.Id, candidate.Attachments);

            // respond
            return createdCandidate;
        }

        public GreenhouseCandidate Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings)
        {
            var binders = new Dictionary<string, IGreenhouseCandidateModelBinder>
            {
                { Constants.GreenhouseFieldTypes.Attachment, new AttachmentModelBinder(settings) },
                { Constants.GreenhouseFieldTypes.LongText, new TextModelBinder() },
                { Constants.GreenhouseFieldTypes.MultiSelect, new MultiSelectValueModelBinder() },
                { Constants.GreenhouseFieldTypes.ShortText, new TextModelBinder() },
                { Constants.GreenhouseFieldTypes.SingleSelect, new SelectedValueModelBinder() }
            };

            var candidate = new GreenhouseCandidate
            {
                Applications = new List<GreenhouseApplication> { new GreenhouseApplication { JobId = posting.JobId } },
                Firstname = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Firstname)),
                Lastname = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Lastname)),
            };


            if (posting.Questions.Any(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Email))
            {
                var emailAddress = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Email));

                if (!string.IsNullOrEmpty(emailAddress))
                {
                    candidate.EmailAddresses.Add(new GreenhouseValueType { Type = "personal", Value = emailAddress });
                }
            }

            if (posting.Questions.Any(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Phone))
            {
                var phone = (string)binders[Constants.GreenhouseFieldTypes.ShortText].Bind(modelState, request, posting.Questions.SingleOrDefault(x => x.Fields.FirstOrDefault()?.Name == Constants.GreenhouseFieldNames.Phone));

                if (!string.IsNullOrEmpty(phone))
                {
                    candidate.PhoneNumbers.Add(new GreenhouseValueType { Type = "personal", Value = phone });
                }
            }

            foreach (var question in posting.Questions.Where(x => !Constants.GreenhouseFieldNames.FixedFields.Contains(x.Fields.FirstOrDefault()?.Name)))
            {
                var field = question.Fields.FirstOrDefault();

                if (field == null)
                {
                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.Attachment)
                {
                    var attachment = (GreenhouseAttachment)binders[Constants.GreenhouseFieldTypes.Attachment].Bind(modelState, request, question);

                    if (attachment != null)
                    {
                        candidate.Attachments.Add(attachment);
                    }

                    continue;
                }

                candidate.CustomFields.Add(new GreenhouseCustomField
                {
                    Name = question.Fields.FirstOrDefault()?.Name,
                    Value = new JValue(binders[field.Type].Bind(modelState, request, question))
                });
            }

            return candidate;
        }
    }

    public interface IGreenhouseApplyService
    {
        Task<GreenhouseCandidateResponse> ApplyAsync(GreenhouseCandidate candidate);
        GreenhouseCandidate Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings);
    }
}
