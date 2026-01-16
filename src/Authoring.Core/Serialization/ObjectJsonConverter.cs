using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authoring.Core.Serialization;

/// <summary>
/// Custom JSON converter for object values that handles JsonElement conversion.
/// </summary>
public class ObjectJsonConverter : JsonConverter<object?>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var longValue))
            {
                // Check if it fits in int
                if (longValue >= int.MinValue && longValue <= int.MaxValue)
                {
                    return (int)longValue;
                }
                return longValue;
            }
            return reader.GetDouble();
        }

        if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }

        if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }

        // For complex types, use JsonDocument
        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.Clone();
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Use JsonSerializer to write the value, which will handle the type appropriately
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
