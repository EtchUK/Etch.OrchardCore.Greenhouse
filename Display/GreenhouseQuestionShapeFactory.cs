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

        public async Task<IList<dynamic>> CreateAsync(GreenhouseJobPosting posting)
        {
            var shapes = new List<dynamic>();

            foreach (var question in posting.Questions.Where(x => !x.Private))
            {
                if (question.Type == Constants.GreenhouseFieldTypes.Attachment)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__Attachment(question));
                    continue;
                }

                if (question.Type == Constants.GreenhouseFieldTypes.Boolean)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__Boolean(question));
                    continue;
                }

                if (question.Type == Constants.GreenhouseFieldTypes.LongText)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__LongText(question));
                    continue;
                }

                if (question.Type == Constants.GreenhouseFieldTypes.MultiSelect)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__MultiSelect(question));
                    continue;
                }

                if (question.Type == Constants.GreenhouseFieldTypes.ShortText)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__ShortText(question));
                    continue;
                }

                if (question.Type == Constants.GreenhouseFieldTypes.SingleSelect)
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__SingleSelect(question));
                    continue;
                }

                _logger.LogWarning($"Unsupported Greenhouse question: {question.Type}");
            }

            return shapes;
        }
    }

    public interface IGreenhouseQuestionShapeFactory
    {
        Task<IList<dynamic>> CreateAsync(GreenhouseJobPosting posting);
    }
}
