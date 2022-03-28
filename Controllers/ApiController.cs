using Etch.OrchardCore.Greenhouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Lucene;
using OrchardCore.Queries;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse.Controllers
{
    [Route("api/greenhouse")]
    [ApiController]
    public class ApiController : Controller
    {
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IDisplayHelper _displayHelper;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly LuceneQuerySource _luceneQuerySource;
        private readonly IQueryManager _queryManager;
        private readonly IUpdateModelAccessor _updateModelAccessor;

        public ApiController(
            IContentItemDisplayManager contentItemDisplayManager,
            IDisplayHelper displayHelper,
            HtmlEncoder htmlEncoder,
            LuceneQuerySource luceneQuerySource,
            IQueryManager queryManager,
            IUpdateModelAccessor updateModelAccessor)
        {
            _contentItemDisplayManager = contentItemDisplayManager;
            _displayHelper = displayHelper;
            _htmlEncoder = htmlEncoder;
            _luceneQuerySource = luceneQuerySource;
            _queryManager = queryManager;
            _updateModelAccessor = updateModelAccessor;
        }

        [HttpGet]
        [Route("jobs")]
        public async Task<IActionResult> Content([FromQuery] GreenhouseQueryViewModel parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Query))
            {
                return BadRequest();
            }

            var luceneQuery = (await _queryManager.GetQueryAsync(parameters.Query)) as LuceneQuery;

            if (luceneQuery == null)
            {
                return BadRequest();
            }

            var queryParameters = new Dictionary<string, object>
            {
                { "department", parameters.Department },
                { "excludedIds", parameters.ExcludedIds },
                { "location", parameters.Location }
            };

            var model = new GreenhouseQueryResultViewModel
            {
                From = parameters.From > 0 ? parameters.From : 0,
                Size = parameters.Size > 0 ? parameters.Size : 1,
                TotalItems = (await _luceneQuerySource.ExecuteQueryAsync(luceneQuery, queryParameters)).Items.Count()
            };

            queryParameters.Add("from", model.From);
            queryParameters.Add("size", model.Size);

            foreach (var contentItem in (await _luceneQuerySource.ExecuteQueryAsync(luceneQuery, queryParameters)).Items)
            {
                var sw = new StringWriter();

                (await _displayHelper.ShapeExecuteAsync(
                        await _contentItemDisplayManager.BuildDisplayAsync((ContentItem)contentItem, _updateModelAccessor.ModelUpdater, "Summary"))).WriteTo(sw, _htmlEncoder);

                model.Items.Add(sw.ToString());
            }

            return Json(model);
        }
    }
}
