using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Display
{
    public class GreenhouseQuestionShapeFactory : IGreenhouseQuestionShapeFactory
    {
        private readonly ILogger<GreenhouseQuestionShapeFactory> _logger;
        private readonly IShapeFactory _shapeFactory;

        public GreenhouseQuestionShapeFactory(ILogger<GreenhouseQuestionShapeFactory> logger, IShapeFactory shapeFactory)
        {
            _logger = logger;
            _shapeFactory = shapeFactory;
        }

        public async Task<IList<dynamic>> CreateAsync(GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings)
        {
            var shapes = new List<dynamic>();

            foreach (var question in posting.Questions)
            {
                var field = question.Fields.FirstOrDefault();

                if (field == null)
                {
                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.Attachment)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__Attachment(new GreenhouseQuestionDisplayContext
                    {
                        Fields = question.Fields,
                        FormSettings = settings,
                        Question = question
                    }));

                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.LongText)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__LongText(new GreenhouseQuestionDisplayContext
                    {
                        Fields = question.Fields,
                        FormSettings = settings,
                        Question = question
                    }));
                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.MultiSelect)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__MultiSelect(new GreenhouseQuestionDisplayContext
                    {
                        Fields = question.Fields,
                        FormSettings = settings,
                        Question = question
                    }));
                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.ShortText)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__ShortText(new GreenhouseQuestionDisplayContext
                    {
                        Fields = question.Fields,
                        FormSettings = settings,
                        Question = question
                    }));
                    continue;
                }

                if (field.Type == Constants.GreenhouseFieldTypes.SingleSelect)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__SingleSelect(new GreenhouseQuestionDisplayContext
                    {
                        Fields = question.Fields,
                        FormSettings = settings,
                        Question = question
                    }));
                    continue;
                }

                _logger.LogWarning($"Unsupported Greenhouse question: {field.Type}");
            }

            return shapes;
        }
    }

    public interface IGreenhouseQuestionShapeFactory
    {
        Task<IList<dynamic>> CreateAsync(GreenhouseJobPosting posting, GreenhousePostingFormPartSettings settings);
    }
}
