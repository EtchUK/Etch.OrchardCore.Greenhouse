namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhousePostingFormPartSettingsViewModel
    {
        public string AllowedFileExtensions { get; set; }
        public string ApplicationSuccessUrl { get; set; }
        public long MaxFileSize { get; set; }
        public bool ShowApplicationForm { get; set; }
        public string SubmitButtonLabel { get; set; }
    }
}
