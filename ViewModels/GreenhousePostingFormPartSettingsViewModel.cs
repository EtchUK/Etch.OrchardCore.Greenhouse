﻿namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhousePostingFormPartSettingsViewModel
    {
        public string AllowedFileExtensions { get; set; }
        public string ApplicationErrorMessage { get; set; }
        public string ApplicationSuccessUrl { get; set; }
        public bool HasReCaptchaEnabled { get; set; }
        public long MaxFileSize { get; set; }
        public bool ShowApplicationForm { get; set; }
        public string SubmitButtonLabel { get; set; }
        public bool UseReCaptcha { get; set; }
        public string ValidationErrorsMessage { get; set; }

    }
}
