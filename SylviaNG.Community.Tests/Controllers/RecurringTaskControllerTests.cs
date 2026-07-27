using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class RecurringTaskControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RecurringTaskController _controller;

    public RecurringTaskControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new RecurringTaskController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<RecurringTaskCreateCommand>(), default)).ReturnsAsync(1L);

        // Act
        var result = await _controller.Create(new RecurringTaskCreateRequest
        {
            Frequency = "Weekly",
            IntervalValue = 1,
            StartDate = DateTime.UtcNow
        });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(1L);
    }
}
