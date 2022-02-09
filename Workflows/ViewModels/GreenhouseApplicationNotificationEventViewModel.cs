using Etch.OrchardCore.Greenhouse.Services.Dtos;

namespace Etch.OrchardCore.Greenhouse.Workflows.ViewModels
{
    public class GreenhouseApplicationNotificationEventViewModel
    {
        public GreenhouseApplication Application { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
        public string Phase { get; set; }
        public GreenhouseApplicationResponse Response { get; set; }
    }
}
