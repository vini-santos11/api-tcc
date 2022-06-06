using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Converters
{
    public class StringConverter : JsonConverter<string>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var valor = reader.GetString();
                if (valor != null)
                    valor = valor.Trim();
                if (valor == string.Empty)
                    valor = null;

                return Convert.ToString(valor, CultureInfo.CurrentCulture);
            }
            else
            {
                using var document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

    }
}
