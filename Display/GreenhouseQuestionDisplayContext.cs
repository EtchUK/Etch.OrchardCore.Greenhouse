﻿using Etch.OrchardCore.Greenhouse.Models;
using Etch.OrchardCore.Greenhouse.Services.Dtos;

namespace Etch.OrchardCore.Greenhouse.Display
{
    public class GreenhouseQuestionDisplayContext
    {
        public GreenhouseField[] Fields { get; set; }
        public GreenhousePostingFormPartSettings FormSettings { get; set; }
        public GreenhouseQuestion Question { get; set; }
    }
}
