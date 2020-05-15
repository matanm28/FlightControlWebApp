using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Converters
{
        using System.Text.Json;
        using System.Text.RegularExpressions;

        public class JsonNonAlphanumericNamingPolicy : JsonNamingPolicy
        {
                /// <inheritdoc />
                public override string ConvertName(string name)
                {
                        var result = Regex.Replace(name, "[^a-zA-Z0-9]", string.Empty);
                        return result;
                }
        }
}