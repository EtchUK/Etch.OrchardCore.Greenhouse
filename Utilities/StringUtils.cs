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

        public static string GetRadios(IList<string> items, string selectedValue)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<div class=\"radios__item\">");
            builder.AppendLine("<input class=\"radios__input\" type=\"radio\" id=\"department-all\" name=\"department\" value=\"\">");
            builder.AppendLine("<label class=\"label radios__label\" for=\"department-all\">");
            builder.AppendLine("All");
            builder.AppendLine("</label>");
            builder.AppendLine("</div>");

            for (int i = 0; i < items.Count; ++i)
            {
                builder.AppendLine("<div class=\"radios__item\">");
                builder.AppendLine($"<input class=\"radios__input\" type=\"radio\" id=\"department-{i}\" name=\"department\" value=\"{items[i]}\">");
                builder.AppendLine($"<label class=\"label radios__label\" for=\"department-{i}\">");
                builder.AppendLine(items[i]);
                builder.AppendLine("</label>");
                builder.AppendLine("</div>");
            }

            return builder.ToString();
        }
    }
}
