using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Application.Features.ActivityLogs.Queries.ActivityLogGetAllPaged;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class ActivityLogControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ActivityLogController _controller;

    public ActivityLogControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ActivityLogController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<ActivityLogResponse>
        {
            Data = new List<ActivityLogResponse> { new() { ActivityId = 1, EmployeeId = 1, Module = "Team", Action = "Create" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ActivityLogGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
