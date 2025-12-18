using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WAPI.Json;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is null) throw new JsonException("DateOnly string is null");
        return DateOnly.ParseExact(s, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}

public sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var s = reader.GetString();
        if (s is null) return null;
        return DateOnly.ParseExact(s, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is null) { writer.WriteNullValue(); return; }
        writer.WriteStringValue(value.Value.ToString(Format, CultureInfo.InvariantCulture));
    }
}