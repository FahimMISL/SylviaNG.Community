using System.Text;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class FileUploadControllerTests : IDisposable
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly FileUploadController _controller;
    private readonly string _tempContentRoot;

    public FileUploadControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _configurationMock = new Mock<IConfiguration>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        // Real temp directory so the happy-path test can exercise actual disk I/O without
        // touching the real wwwroot/uploads folder; cleaned up in Dispose().
        _tempContentRoot = Path.Combine(Path.GetTempPath(), "SylviaNGCommunityTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempContentRoot);
        _environmentMock.Setup(e => e.ContentRootPath).Returns(_tempContentRoot);

        _configurationMock.Setup(c => c["FileStorage:LocalRootPath"]).Returns("wwwroot/uploads");
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(42);

        _controller = new FileUploadController(
            _mediatorMock.Object,
            _environmentMock.Object,
            _configurationMock.Object,
            _currentUserServiceMock.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempContentRoot))
        {
            Directory.Delete(_tempContentRoot, recursive: true);
        }
    }

    private static IFormFile BuildFormFile(string fileName, string content = "fake-bytes", string contentType = "image/png")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task Upload_WithEmptyFile_ShouldReturnBadRequest()
    {
        // Arrange
        var emptyStream = new MemoryStream();
        var file = new FormFile(emptyStream, 0, 0, "file", "empty.png");

        // Act
        var result = await _controller.Upload(file, "Post", null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task Upload_WithFileExceedingMaxSize_ShouldReturnBadRequest()
    {
        // Arrange - 11 MB, one over the 10 MB image limit
        var oversizedContent = new string('a', 11 * 1024 * 1024);
        var file = BuildFormFile("big.png", oversizedContent);

        // Act
        var result = await _controller.Upload(file, "Post", null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        badRequest.Value.Should().BeOfType<string>().Which.Should().Contain("maximum allowed size");
        _mediatorMock.Verify(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task Upload_WithVideoOverImageLimitButUnderVideoLimit_ShouldSucceed()
    {
        // Arrange - 20 MB: over the 10 MB image cap, but videos get a 150 MB cap.
        var content = new string('a', 20 * 1024 * 1024);
        var file = BuildFormFile("clip.mp4", content, contentType: "video/mp4");
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default)).ReturnsAsync(7L);

        // Act
        var result = await _controller.Upload(file, "Post", null);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Upload_WithImageOverVideoTierButUnderNothing_ShouldReturnBadRequest()
    {
        // Arrange - images are capped at 10 MB even though videos/documents allow more.
        var content = new string('a', 15 * 1024 * 1024);
        var file = BuildFormFile("photo.jpg", content);

        // Act
        var result = await _controller.Upload(file, "Post", null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        badRequest.Value.Should().BeOfType<string>().Which.Should().Contain("10 MB");
    }

    [Fact]
    public async Task Upload_WithDisallowedExtension_ShouldReturnBadRequest()
    {
        // Arrange
        var file = BuildFormFile("malicious.exe", contentType: "application/octet-stream");

        // Act
        var result = await _controller.Upload(file, "Post", null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        badRequest.Value.Should().BeOfType<string>().Which.Should().Contain("not allowed");
        _mediatorMock.Verify(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default), Times.Never);
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".JPEG")]
    [InlineData(".png")]
    [InlineData(".gif")]
    [InlineData(".webp")]
    [InlineData(".mp4")]
    [InlineData(".mov")]
    [InlineData(".webm")]
    public async Task Upload_WithAllowedExtension_ShouldWriteFileAndReturnOkWithMetadata(string extension)
    {
        // Arrange
        var fileName = $"photo{extension}";
        var file = BuildFormFile(fileName);
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default)).ReturnsAsync(7L);

        // Act
        var result = await _controller.Upload(file, "Post", 123);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<FileUploadResponse>().Subject;
        response.FileId.Should().Be(7L);
        response.OriginalFileName.Should().Be(fileName);
        response.StoragePath.Should().StartWith("uploads/Post/");
        response.StoragePath.Should().EndWith(extension); // extension case is preserved as-is from the original filename

        // The bytes should actually be on disk under wwwroot/uploads/Post/<yyyy-MM>/<guid>.ext
        var writtenPath = Path.Combine(_tempContentRoot, "wwwroot", "uploads", response.StoragePath.Substring("uploads/".Length));
        File.Exists(writtenPath).Should().BeTrue();

        // UploadedBy must come from ICurrentUserService, never a client-supplied value - verify
        // via the command sent to the mediator.
        _mediatorMock.Verify(m => m.Send(
            It.Is<FileStorageCreateCommand>(cmd =>
                cmd.Request.Module == "Post" &&
                cmd.Request.EntityId == 123 &&
                cmd.Request.OriginalFileName == fileName &&
                cmd.Request.UploadedBy == 42),
            default), Times.Once);
    }

    // NOTE: A test verifying rejection when the module is missing/blank, and a full round trip
    // through a real ASP.NET Core TestServer/WebApplicationFactory (exercising actual model
    // binding of multipart/form-data into [FromForm] IFormFile), are intentionally out of scope
    // here - this suite mirrors the rest of the codebase's convention of pure unit tests against
    // mocked IMediator/IWebHostEnvironment/IConfiguration/ICurrentUserService rather than spinning
    // up a real test server.
    [Fact]
    public async Task Upload_WithBlankModule_ShouldReturnBadRequest()
    {
        // Arrange
        var file = BuildFormFile("photo.png");

        // Act
        var result = await _controller.Upload(file, "  ", null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default), Times.Never);
    }
}
