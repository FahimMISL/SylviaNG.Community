using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentDelete;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Features.PostComments.Queries.PostCommentGetAll;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class PostCommentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PostCommentController _controller;

    public PostCommentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PostCommentController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new List<PostCommentResponse> { new() { CommentId = 1, PostId = 1, Content = "Hi" } };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PostCommentGetAllQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAll(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Add_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<PostCommentAddCommand>(), default)).ReturnsAsync(5L);

        // Act
        var result = await _controller.Add(1, new PostCommentAddRequest { EmployeeId = 2, Content = "Nice" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(5L);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        // Act
        var result = await _controller.Delete(1, 5);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<PostCommentDeleteCommand>(), default), Times.Once);
    }
}
