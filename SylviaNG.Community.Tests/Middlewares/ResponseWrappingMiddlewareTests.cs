using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SylviaNG.Community.Middlewares;

namespace SylviaNG.Community.Tests.Middlewares;

public class ResponseWrappingMiddlewareTests
{
    private static DefaultHttpContext CreateContext(out MemoryStream responseStream)
    {
        var context = new DefaultHttpContext();
        responseStream = new MemoryStream();
        context.Response.Body = responseStream;
        return context;
    }

    [Fact]
    public async Task InvokeAsync_WithJsonResponse_ShouldWrapInEnvelope()
    {
        // Arrange
        var context = CreateContext(out var responseStream);
        var middleware = new ResponseWrappingMiddleware(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            ctx.Response.ContentType = "application/json; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes("{\"foo\":\"bar\"}");
            await ctx.Response.Body.WriteAsync(bytes);
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        responseStream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseStream).ReadToEndAsync();
        context.Response.ContentType.Should().Be("application/json");

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("hasError").GetBoolean().Should().BeFalse();
        doc.RootElement.GetProperty("content").GetProperty("foo").GetString().Should().Be("bar");
    }

    [Fact]
    public async Task InvokeAsync_WithNonJsonResponse_ShouldPassThroughUnwrapped()
    {
        // Arrange - simulates a file-download action (application/octet-stream). Reading raw
        // binary bytes as text and re-serializing them as a JSON string would corrupt the
        // payload and overwrite the real Content-Type/Content-Disposition the action set.
        var context = CreateContext(out var responseStream);
        var fileBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x00, 0xFF, 0x10 };
        var middleware = new ResponseWrappingMiddleware(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            ctx.Response.ContentType = "application/octet-stream";
            ctx.Response.Headers["Content-Disposition"] = "attachment; filename=\"test.bin\"";
            await ctx.Response.Body.WriteAsync(fileBytes);
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert - bytes and headers pass through byte-for-byte, untouched by the JSON envelope.
        responseStream.Seek(0, SeekOrigin.Begin);
        var resultBytes = responseStream.ToArray();
        resultBytes.Should().Equal(fileBytes);
        context.Response.ContentType.Should().Be("application/octet-stream");
        context.Response.Headers["Content-Disposition"].ToString().Should().Contain("test.bin");
    }

    [Fact]
    public async Task InvokeAsync_With204NoContent_ShouldWrapAsEmptySuccessEnvelope()
    {
        // Arrange - existing behavior, unaffected by the new non-JSON skip path.
        var context = CreateContext(out var responseStream);
        var middleware = new ResponseWrappingMiddleware(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        responseStream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseStream).ReadToEndAsync();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("hasError").GetBoolean().Should().BeFalse();
        doc.RootElement.GetProperty("content").ValueKind.Should().Be(JsonValueKind.Null);
    }
}
