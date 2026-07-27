using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceDelete;
using SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Application.Features.DashboardPreferences.Queries.DashboardPreferenceGetAllByEmployee;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class DashboardPreferenceControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DashboardPreferenceController _controller;

    public DashboardPreferenceControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DashboardPreferenceController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetByEmployee_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new List<DashboardPreferenceResponse> { new() { PreferenceId = 1, EmployeeId = 1, WidgetName = "TeamRoster" } };
        _mediatorMock.Setup(m => m.Send(It.IsAny<DashboardPreferenceGetAllByEmployeeQuery>(), default)).ReturnsAsync(expected);

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
        _mediatorMock.Setup(m => m.Send(It.IsAny<DashboardPreferenceUpsertCommand>(), default)).ReturnsAsync(7L);

        // Act
        var result = await _controller.Upsert(new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "TeamRoster" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(7L);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DashboardPreferenceDeleteCommand>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
