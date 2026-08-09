using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class AuditLogServiceTests
{
    private readonly Mock<IAuditLogRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AuditLogService _service;

    public AuditLogServiceTests()
    {
        _repositoryMock = new Mock<IAuditLogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new AuditLogService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task LogAsync_ShouldAddEntryAndReturnId()
    {
        // Arrange
        var request = new AuditLogCreateRequest { TableName = "Teams", RecordId = 1, Action = "Update", PerformedBy = 2 };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
            .Callback<AuditLog>(a => a.AuditId = 1);

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
        var pagedResult = new PagedResult<AuditLog>
        {
            Data = new List<AuditLog> { new() { AuditId = 1, TableName = "Teams", RecordId = 1, Action = "Update", PerformedBy = 2 } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _repositoryMock.Setup(r => r.GetPaginatedAsync(request)).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedAsync(request);

        // Assert
        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(a => a.AuditId == 1);
    }
}
