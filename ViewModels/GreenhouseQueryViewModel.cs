namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhouseQueryViewModel
    {
        public string Query { set; get; }

        public string Department { get; set; }
        public string[] ExcludedIds { get; set; }
        public int From { set; get; }
        public string Location { get; set; }
        public int Size { get; set; }
    }
}
