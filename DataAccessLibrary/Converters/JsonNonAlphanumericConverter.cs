using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Converters
{
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using DataAccessLibrary.Models;

        class JsonNonAlphanumericConverter<T> : JsonConverter<T>
        {

                /// <inheritdoc />
                public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                        JsonSerializerOptions myOptions = options;
                        myOptions.PropertyNamingPolicy = new JsonNonAlphanumericNamingPolicy();
                        return JsonSerializer.Deserialize<T>(ref reader, myOptions);
                }

                /// <inheritdoc />
                public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
                {
                        JsonSerializer.Serialize<T>(writer, value, options);
                }
        }
}