namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhousePostingFormPartSettings
    {
        public string[] AllowedFileExtensions { get; set; } = Constants.Defaults.AllowedFileExtensions;
        public string ApplicationSuccessUrl { get; set; }
        public long MaxFileSize { get; set; } = Constants.Defaults.MaxFileSize;
        public bool ShowApplicationForm { get; set; }
        public string SubmitButtonLabel { get; set; }
    }
}
