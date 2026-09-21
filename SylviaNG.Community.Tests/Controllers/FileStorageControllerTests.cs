using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetAllPaged;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class FileStorageControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly FileStorageController _controller;

    public FileStorageControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(42);
        _currentUserServiceMock.Setup(c => c.RequireEmployeeId()).Returns(42);
        _controller = new FileStorageController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new FileStorageResponse { FileId = 1, Module = "Team", FileName = "a.png", OriginalFileName = "a.png", StoragePath = "/x", UploadedBy = 1 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageGetByIdQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<FileStorageResponse>
        {
            Data = new List<FileStorageResponse> { new() { FileId = 1, Module = "Team", FileName = "a.png", OriginalFileName = "a.png", StoragePath = "/x", UploadedBy = 1 } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest(), null, null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default)).ReturnsAsync(9L);

        // Act
        var result = await _controller.Create(new FileStorageCreateRequest
        {
            Module = "Team",
            FileName = "a.png",
            OriginalFileName = "a.png",
            StoragePath = "/x",
            UploadedBy = 1
        });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(9L);
    }

    [Fact]
    public async Task Create_ShouldOverrideUploadedByFromCurrentUser_NotClientInput()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<FileStorageCreateCommand>(), default)).ReturnsAsync(9L);

        // Act - the client attempts to spoof a different employee as the uploader
        await _controller.Create(new FileStorageCreateRequest
        {
            Module = "Team",
            FileName = "a.png",
            OriginalFileName = "a.png",
            StoragePath = "/x",
            UploadedBy = 999
        });

        // Assert - UploadedBy must come from ICurrentUserService (42), never the request body
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<FileStorageCreateCommand>(cmd => cmd.Request.UploadedBy == 42),
                default),
            Times.Once);
    }
}
