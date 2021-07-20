using System.Collections.Generic;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public class ModelStateValueDto
    {
        public string AttemptedValue { get; set; }
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
        public string Key { get; set; }
        public object RawValue { get; set; }
    }
}
