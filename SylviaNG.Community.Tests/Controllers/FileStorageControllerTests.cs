using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetAllPaged;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class FileStorageControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly FileStorageController _controller;

    public FileStorageControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new FileStorageController(_mediatorMock.Object);
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
        var result = await _controller.GetPaged(new PagedRequest(), null, null);

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
}
