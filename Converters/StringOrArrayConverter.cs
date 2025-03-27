using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etch.OrchardCore.Greenhouse.Converters
{
    public class StringOrArrayConverter : JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return [reader.GetString()];
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                List<string> values = [];

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        values.Add(reader.GetString());
                    }
                }

                return [.. values];
            }

            return [];
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
