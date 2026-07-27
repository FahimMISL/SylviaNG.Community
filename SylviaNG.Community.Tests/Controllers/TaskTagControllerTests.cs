using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class TaskTagControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TaskTagController _controller;

    public TaskTagControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TaskTagController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<TaskTagCreateCommand>(), default)).ReturnsAsync(1L);

        // Act
        var result = await _controller.Create(new TaskTagCreateRequest { Name = "Urgent" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(1L);
    }
}
