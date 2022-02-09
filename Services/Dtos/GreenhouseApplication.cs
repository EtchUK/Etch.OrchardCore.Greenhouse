using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseApplication
    {
        public GreenhouseAttachment Cover { get; set; }
        public string CoverText { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public IList<GreenhouseCustomField> Questions { get; set; } = new List<GreenhouseCustomField>();
        public GreenhouseAttachment Resume { get; set; }
        public string ResumeText { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var data = new Dictionary<string, object>
            {
                { "email", Email },
                { "first_name", FirstName },
                { "last_name", LastName },
                { "location", Location },
                { "phone", Phone },
            };

            if (Cover != null)
            {
                data.Add("cover_letter_content", Cover.Content);
                data.Add("cover_letter_content_filename", Cover.Filename);
            }

            if (!string.IsNullOrWhiteSpace(CoverText))
            {
                data.Add("cover_letter_text", CoverText);
            }

            if (Resume != null)
            {
                data.Add("resume_content", Resume.Content);
                data.Add("resume_content_filename", Resume.Filename);
            }

            if (!string.IsNullOrWhiteSpace(ResumeText))
            {
                data.Add("resume_letter_text", ResumeText);
            }

            foreach (var question in Questions)
            {
                if (question.Name.EndsWith("[]"))
                {
                    data.Add(question.Name.Replace("[]", ""), question.Value);
                    continue;
                }

                data.Add(question.Name, question.Value);
            }

            return data;
        }
    }
}
