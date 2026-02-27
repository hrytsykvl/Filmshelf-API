using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.JsonConverters;
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateStr = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateStr))
                return null;

            if (DateTime.TryParse(dateStr, out var date))
                return date;
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        throw new JsonException("Invalid date format.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
        else
            writer.WriteNullValue();
    }
}
