﻿namespace Etch.OrchardCore.Greenhouse
{
    public static class Constants
    {
        public const string GroupId = "Greenhouse";

        public static class Defaults
        {
            private static readonly string[] _allowedFileExtensions = new string[] { ".docx", ".doc", ".pdf" };

            public static string[] AllowedFileExtensions { get => _allowedFileExtensions; }
            public const string ApiHostname = "https://harvest.greenhouse.io/v1";
            public const string Author = "Greenhouse";
            public const string ContentType = "GreenhousePosting";
            public const long MaxFileSize = 2097152;
        }

        public static class GreenhouseApplicationPhases
        {
            public const string Apply = "Apply";
            public const string Validation = "Validation";
        }

        public static class GreenhouseFieldNames
        {
            private static readonly string[] _fixedFields = new string[] { Email, Firstname, Lastname, Phone };

            public const string Email = "email";
            public const string Firstname = "first_name";
            public const string Lastname = "last_name";
            public const string Phone = "phone";

            public static string[] FixedFields { get; set; } = _fixedFields;
        }

        public static class GreenhouseFieldTypes
        {
            public const string Attachment = "input_file";
            public const string LongText = "textarea";
            public const string MultiSelect = "multi_value_multi_select";
            public const string ShortText = "input_text";
            public const string SingleSelect = "multi_value_single_select";
        }
    }
}
