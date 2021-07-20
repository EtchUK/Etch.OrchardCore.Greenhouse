namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhouseSettings
    {
        public string[] AllowedFileExtensions { get; set; } = Constants.Defaults.AllowedFileExtensions;
        public string ApiHostname { get; set; } = Constants.Defaults.ApiHostname;
        public string ApiKey { get; set; }
        public string DefaultSuccessUrl { get; set; }
        public long MaxFileSize { get; set; } = Constants.Defaults.MaxFileSize;
        public long OnBehalfOfId { get; set; }
    }
}
