namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhouseSettings
    {
        public string ApiHostname { get; set; } = Constants.Defaults.ApiHostname;
        public string ApiKey { get; set; }
        public string DefaultSuccessUrl { get; set; }
        public long OnBehalfOfId { get; set; }
    }
}
