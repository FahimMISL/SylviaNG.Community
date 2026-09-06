using System.Text.Json;
using FluentAssertions;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Tests.Utils;

/// <summary>
/// Regression coverage for a real bug: the frontend sends true-UTC, "Z"-suffixed timestamps
/// (Date#toISOString()), but this converter used to always reinterpret the digits as business-local
/// (Asia/Dhaka, UTC+6) time regardless, silently shifting every Election StartDate/EndDate 6 hours
/// earlier than intended in the database - invisible on a naive round-trip through the same API,
/// since the write side re-applies the opposite shift, but very real for anything (vote-window
/// checks, auto-close) that compares DateTime.UtcNow against the stored value directly.
/// </summary>
public class LocalDateTimeJsonConverterTests : IDisposable
{
    public LocalDateTimeJsonConverterTests()
    {
        // DateTimeUtility is a static, process-wide singleton normally initialized once at startup
        // (Infrastructure/Extensions/DependencyInjection.cs) from RegionalSettings:TimezoneId, and
        // every other test in this suite implicitly relies on it staying at its compile-time
        // default (TimeZoneInfo.Utc) - set it to a real, non-UTC business zone only for the
        // duration of each test here, and restore it in Dispose() so this doesn't leak into any
        // other test class in the same process.
        DateTimeUtility.Initialize("Asia/Dhaka");
    }

    public void Dispose()
    {
        DateTimeUtility.Initialize("UTC");
    }

    private static JsonSerializerOptions Options() => new() { Converters = { new LocalDateTimeJsonConverter() } };

    private class Wrapper
    {
        public DateTime Value { get; set; }
    }

    [Fact]
    public void Read_WhenIncomingIsUtcZSuffixed_ShouldNotDoubleConvert()
    {
        var json = "{\"Value\":\"2026-09-05T12:00:00.000Z\"}";

        var result = JsonSerializer.Deserialize<Wrapper>(json, Options())!;

        result.Value.Should().Be(new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Read_WhenIncomingIsBareOffsetless_ShouldConvertFromBusinessLocalTime()
    {
        var json = "{\"Value\":\"2026-09-05T19:12:00\"}";

        var result = JsonSerializer.Deserialize<Wrapper>(json, Options())!;

        result.Value.Should().Be(new DateTime(2026, 9, 5, 13, 12, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Read_WhenTrueUtcRoundTripsThroughWrite_ShouldReproduceSameInstant()
    {
        var original = new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc);
        var json = "{\"Value\":\"" + original.ToString("O") + "\"}";

        var result = JsonSerializer.Deserialize<Wrapper>(json, Options())!;

        result.Value.Should().Be(original);
    }
}
