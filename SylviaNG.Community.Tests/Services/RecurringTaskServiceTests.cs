using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class RecurringTaskServiceTests
{
    private readonly Mock<IRecurringTaskRepository> _recurringTaskRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RecurringTaskService _service;

    public RecurringTaskServiceTests()
    {
        _recurringTaskRepositoryMock = new Mock<IRecurringTaskRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new RecurringTaskService(_recurringTaskRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new RecurringTaskCreateRequest { Frequency = "Weekly", IntervalValue = 1, StartDate = DateTime.UtcNow };
        _recurringTaskRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RecurringTask>()))
            .Callback<RecurringTask>(rt => rt.RecurringTaskId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recurringTaskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((RecurringTask?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldDeactivateRatherThanHardDelete()
    {
        // Arrange - US-7.12: cancelling the series stops future generation but must not remove
        // the row, since already-generated Task rows point back to it via RecurringTaskId.
        var entity = new RecurringTask { RecurringTaskId = 1, Frequency = "Weekly", IntervalValue = 1, IsActive = true };
        _recurringTaskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        entity.IsActive.Should().BeFalse();
        _recurringTaskRepositoryMock.Verify(r => r.Update(entity), Times.Once);
        _recurringTaskRepositoryMock.Verify(r => r.Delete(It.IsAny<RecurringTask>()), Times.Never);
    }
}
