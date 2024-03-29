﻿namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhouseSettingsViewModel
    {
        public string AllowedFileExtensions { get; set; }
        public string ApiKey { get; set; }
        public string BoardToken { get; set; }
        public string DefaultSuccessUrl { get; set; }
        public long MaxFileSize { get; set; }
    }
}
