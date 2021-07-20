using Microsoft.AspNetCore.Mvc;

namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhouseApplyViewModel
    {
        [BindProperty(Name = "email")]
        public string Email { get; set; }

        [BindProperty(Name = "first_name")]
        public string Firstname { get; set; }

        [BindProperty(Name = "last_name")]
        public string Lastname { get; set; }
    }
}
