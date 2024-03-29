﻿using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;
using OrchardCore.ContentManagement;
using OrchardCore.ReCaptcha.Configuration;
using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhousePostingFormPartViewModel
    {
        public ContentItem ContentItem { get; set; }
        public GreenhouseJob Job { get; set; }
        public GreenhouseJobPosting Posting { get; set; }
        public GreenhousePostingPart PostingPart { get; set; }
        public IList<dynamic> Questions { get; set; }
        public ReCaptchaSettings ReCaptchaSettings { get; set; }
        public GreenhousePostingFormPartSettings Settings { get; set; }
        public bool ShowApplicationForm {get; set; }
        public bool UseReCaptcha { get; set; }
    }
}
