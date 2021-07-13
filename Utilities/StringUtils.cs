using System.Collections.Generic;
using System.Text;

namespace Etch.OrchardCore.Greenhouse.Utilities
{
    public static class StringUtils
    {
        public static string GetOptions(IList<string> items, string selectedValue)
        {
            var builder = new StringBuilder();
            builder.Append("<option value=\"\">All</option>\n");

            for (int i = 0; i < items.Count; ++i)
            {
                builder.Append(string.Format("<option value=\"{0}\"{1}>{2}</option>\n",
                                items[i],
                                !string.IsNullOrEmpty(selectedValue) && selectedValue == items[i] ? " selected=\"selected\"" : "",
                                items[i]));
            }

            return builder.ToString();
        }
    }
}
