using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.ViewModels
{
    public class GreenhouseQueryResultViewModel
    {
        public int From { get; set; }
        public IList<string> Items { get; set; } = new List<string>();
        public int Size { get; set; }
        public int TotalItems { get; set; }
    }
}
