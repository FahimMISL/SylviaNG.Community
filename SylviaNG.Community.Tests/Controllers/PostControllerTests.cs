using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Posts.Commands.PostCreate;
using SylviaNG.Community.Application.Features.Posts.Commands.PostDelete;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked;
using SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class PostControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PostController _controller;

    public PostControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PostController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new PostResponse { PostId = 1, Type = "Update", Visibility = "Public" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PostGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<PostResponse>
        {
            Data = new List<PostResponse> { new() { PostId = 1 } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PostGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<PostCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new PostCreateRequest { EmployeeId = 1, Type = "Update", Visibility = "Public" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        // Act
        var result = await _controller.Update(1, new PostUpdateRequest { Content = "Edited" });

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<PostUpdateCommand>(), default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<PostDeleteCommand>(), default), Times.Once);
    }

    [Fact]
    public async Task SetLocked_ShouldReturnOk()
    {
        // Act
        var result = await _controller.SetLocked(1, true);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<PostSetLockedCommand>(), default), Times.Once);
    }

    [Fact]
    public async Task SetHidden_ShouldReturnOk()
    {
        // Act
        var result = await _controller.SetHidden(1, true);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<PostSetHiddenCommand>(), default), Times.Once);
    }
}
