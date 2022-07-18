namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhousePostingFormPartSettings
    {
        public string[] AllowedFileExtensions { get; set; } = Constants.Defaults.AllowedFileExtensions;
        public string ApplicationErrorMessage { get; set; }
        public string ApplicationSuccessUrl { get; set; }
        public long MaxFileSize { get; set; } = Constants.Defaults.MaxFileSize;
        public bool ShowApplicationForm { get; set; }
        public string SubmitButtonLabel { get; set; }
        public bool UseReCaptcha { get; set; }
        public string ValidationErrorsMessage { get; set; }
    }
}
