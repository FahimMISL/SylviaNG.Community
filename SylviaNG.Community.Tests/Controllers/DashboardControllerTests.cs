using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Features.Dashboard.Queries.AdminDashboardSummaryGet;
using SylviaNG.Community.Application.Features.Dashboard.Queries.EmployeeDashboardSummaryGet;
using SylviaNG.Community.Application.Features.Dashboard.Queries.SupervisorTaskOverviewGet;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new DashboardController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetEmployeeSummary_ShouldUseCallerEmployeeIdFromCurrentUserService()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.EmployeeId).Returns(7);
        var expected = new EmployeeDashboardSummaryResponse { TeamCount = 2 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeDashboardSummaryGetQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetEmployeeSummary();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        _mediatorMock.Verify(m => m.Send(
            It.Is<EmployeeDashboardSummaryGetQuery>(q => q.EmployeeId == 7), default), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeSummary_WhenCallerHasNoEmployeeRecord_ShouldThrowUnauthorizedException()
    {
        // Arrange - e.g. an Admin-type caller, not an Employee record.
        _currentUserServiceMock.Setup(s => s.EmployeeId).Returns((long?)null);

        // Act
        var act = () => _controller.GetEmployeeSummary();

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task GetSupervisorTaskOverview_ShouldUseCallerEmployeeIdFromCurrentUserService()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.EmployeeId).Returns(9);
        var expected = new SupervisorTaskOverviewResponse { Total = 5, Overdue = 1 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SupervisorTaskOverviewGetQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetSupervisorTaskOverview();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        _mediatorMock.Verify(m => m.Send(
            It.Is<SupervisorTaskOverviewGetQuery>(q => q.SupervisorEmployeeId == 9), default), Times.Once);
    }

    [Fact]
    public async Task GetSupervisorTaskOverview_WhenCallerHasNoEmployeeRecord_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.EmployeeId).Returns((long?)null);

        // Act
        var act = () => _controller.GetSupervisorTaskOverview();

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task GetAdminSummary_ShouldReturnOkWithMediatorResult()
    {
        // Arrange
        var expected = new AdminDashboardSummaryResponse { ActiveSurveyCount = 3, PendingListingCount = 2 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<AdminDashboardSummaryGetQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAdminSummary();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
