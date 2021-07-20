using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Etch.OrchardCore.Greenhouse.Extensions
{
    public static class FormFileExtensions
    {
        public static string ToBase64(this IFormFile file)
        {
            if (file.Length == 0)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            file.OpenReadStream().CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
