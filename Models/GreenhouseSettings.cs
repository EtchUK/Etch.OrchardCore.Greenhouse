namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhouseSettings
    {
        public string ApiHostname { get; set; } = Constants.Defaults.ApiHostname;
        public string ApiKey { get; set; }
        public string BoardToken { get; set; }
        public long OnBehalfOfId { get; set; }

        public string BoardUrl
        {
            get
            {
                return $"https://boards.greenhouse.io/{BoardToken}";
            }
        }
    }
}
