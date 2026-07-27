using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ActivityLogServiceTests
{
    private readonly Mock<IActivityLogRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ActivityLogService _service;

    public ActivityLogServiceTests()
    {
        _repositoryMock = new Mock<IActivityLogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ActivityLogService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task LogAsync_ShouldAddEntryAndReturnId()
    {
        // Arrange
        var request = new ActivityLogCreateRequest { EmployeeId = 1, Module = "Team", Action = "Create", EntityType = "Team", EntityId = 10 };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ActivityLog>()))
            .Callback<ActivityLog>(a => a.ActivityId = 1);

        // Act
        var result = await _service.LogAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var request = new PagedRequest();
        var pagedResult = new PagedResult<ActivityLog>
        {
            Data = new List<ActivityLog> { new() { ActivityId = 1, EmployeeId = 1, Module = "Team", Action = "Create" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _repositoryMock.Setup(r => r.GetPaginatedAsync(request)).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedAsync(request);

        // Assert
        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(a => a.ActivityId == 1);
    }
}
