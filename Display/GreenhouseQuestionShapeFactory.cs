using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
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
                if (question.Type == "attachment")
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__Attachment(question));
                    continue;
                }

                if (question.Type == "boolean")
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__Boolean(question));
                    continue;
                }

                if (question.Type == "long_text")
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__LongText(question));
                    continue;
                }

                if (question.Type == "multi_select")
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion_MultiSelect(question));
                    continue;
                }

                if (question.Type == "short_text")
                {
                    shapes.Add(await _shapeFactory.New.GreenhouseQuestion__ShortText(question));
                    continue;
                }

                if (question.Type == "single_select")
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
