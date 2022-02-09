namespace Etch.OrchardCore.Greenhouse.Models
{
    public class GreenhouseSettings
    {
        public string ApiKey { get; set; }
        public string BoardToken { get; set; }

        public string BoardUrl
        {
            get
            {
                return $"https://boards.greenhouse.io/{BoardToken}";
            }
        }
    }
}
