using System.Text.Json;
using System.Text.Json.Serialization;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.SharedKernel.Utils;

/// <summary>
/// Automatically converts UTC DateTime to local time when serializing to JSON,
/// and treats incoming JSON DateTime as local time (converts to UTC) - but only when the
/// incoming string is genuinely bare/offset-less (DateTimeKind.Unspecified). A "Z"/offset-suffixed
/// string (e.g. the frontend's Date#toISOString()) already represents an unambiguous absolute
/// instant; reinterpreting its digits as business-local time on top of that would double-convert
/// it, silently shifting every StartDate/EndDate by the business timezone's UTC offset.
/// </summary>
public class LocalDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dt = reader.GetDateTime();
        if (dt == default) return default;

        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTimeUtility.ConvertLocalToUtc(dt)
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var local = value.Kind == DateTimeKind.Utc
            ? DateTimeUtility.ConvertUtcToLocal(value)
            : value;

        writer.WriteStringValue(local.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}

public class NullableLocalDateTimeJsonConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        var dt = reader.GetDateTime();
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTimeUtility.ConvertLocalToUtc(dt)
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var local = value.Value.Kind == DateTimeKind.Utc
            ? DateTimeUtility.ConvertUtcToLocal(value.Value)
            : value.Value;

        writer.WriteStringValue(local.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
