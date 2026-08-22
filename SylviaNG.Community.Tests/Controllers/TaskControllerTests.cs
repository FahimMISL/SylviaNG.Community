using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAllPaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class TaskControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new TaskController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new TaskResponse { TaskId = 1, Title = "Prepare report" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<TaskGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<TaskResponse>
        {
            Data = new List<TaskResponse> { new() { TaskId = 1, Title = "Prepare report" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<TaskGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new TaskFilterRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<TaskCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new TaskCreateRequest
        {
            TeamId = 1,
            AssignedBy = 2,
            AssignedTo = 3,
            Title = "Prepare report",
            Priority = "High",
            Status = "Open"
        });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task Update_ShouldSendChangedByFromCurrentUser()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.EmployeeId).Returns(7);

        // Act
        var result = await _controller.Update(1, new TaskUpdateRequest { Status = "Completed" });

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(
            It.Is<Application.Features.Tasks.Commands.TaskUpdate.TaskUpdateCommand>(c => c.TaskId == 1 && c.ChangedBy == 7),
            default), Times.Once);
    }
}
