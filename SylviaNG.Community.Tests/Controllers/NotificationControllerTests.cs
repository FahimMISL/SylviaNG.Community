using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkRead;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetAllPaged;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new NotificationController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new NotificationResponse { NotificationId = 1, EmployeeId = 1, Title = "Welcome" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<NotificationResponse>
        {
            Data = new List<NotificationResponse> { new() { NotificationId = 1, EmployeeId = 1, Title = "Welcome" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(1, new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new NotificationCreateRequest { EmployeeId = 1, Title = "Welcome" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task MarkAsRead_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<NotificationMarkReadCommand>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.MarkAsRead(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
