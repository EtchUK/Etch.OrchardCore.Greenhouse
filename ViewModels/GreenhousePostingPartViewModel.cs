using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using OrchardCore.ContentManagement;

namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhousePostingPartViewModel
    {
        public ContentItem ContentItem { get; set; }
        public GreenhouseJob Job { get; set; }
        public GreenhousePostingPart Part { get; set; }
        public GreenhouseJobPosting Posting { get; set; }
        public bool ShowApplicationForm { get; set; }
    }
}
