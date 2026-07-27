using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationPreferenceGetAllByEmployee;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class NotificationPreferenceControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NotificationPreferenceController _controller;

    public NotificationPreferenceControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new NotificationPreferenceController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetByEmployee_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new List<NotificationPreferenceResponse> { new() { PreferenceId = 1, EmployeeId = 1, Category = "Announcements" } };
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationPreferenceGetAllByEmployeeQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetByEmployee(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Upsert_ShouldReturnOkWithId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationPreferenceUpsertCommand>(), default)).ReturnsAsync(5L);

        // Act
        var result = await _controller.Upsert(new NotificationPreferenceUpsertRequest { EmployeeId = 1, Category = "Announcements" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(5L);
    }
}
