namespace Etch.OrchardCore.Greenhouse.Services.Options
{
    public class GreenhouseSyncOptions
    {
        public string Author { get; set; }
        public string ContentType { get; set; }
        public bool ExternalOnly { get; set; }
        public string[] Locations { get; set; }
        public bool PreventDuplicatePostingsForSameJob { get; set; }
        public string UrlPrefix { get; set; }
    }
}
