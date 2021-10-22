namespace Etch.OrchardCore.Greenhouse.Workflows.ViewModels
{
    public class SyncGreenhousePostingsTaskViewModel
    {
        public string Author { get; set; }
        public string ContentType { get; set; }
        public bool ExternalOnly { get; set; }
        public string Locations { get; set; }
        public bool PreventDuplicatePostingsForSameJob { get; set; }
        public string UrlPrefix { get; set; }
    }
}
