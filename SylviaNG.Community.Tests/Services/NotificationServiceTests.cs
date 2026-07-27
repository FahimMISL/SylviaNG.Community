using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new NotificationService(_notificationRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new NotificationCreateRequest { EmployeeId = 1, Title = "Welcome" };

        _notificationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Callback<Notification>(n => n.NotificationId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task MarkAsReadAsync_WhenExists_ShouldSetIsReadAndReadAt()
    {
        // Arrange
        var notification = new Notification { NotificationId = 1, EmployeeId = 1, Title = "Welcome", IsRead = false };
        _notificationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notification);

        // Act
        await _service.MarkAsReadAsync(1);

        // Assert
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        _notificationRepositoryMock.Verify(r => r.Update(notification), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task MarkAsReadAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _notificationRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Notification?)null);

        // Act
        var act = () => _service.MarkAsReadAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _notificationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Notification?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var employeeId = 1L;
        var request = new PagedRequest();
        var pagedResult = new PagedResult<Notification>
        {
            Data = new List<Notification> { new() { NotificationId = 1, EmployeeId = employeeId, Title = "Welcome" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _notificationRepositoryMock.Setup(r => r.GetPaginatedByEmployeeAsync(employeeId, request)).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedAsync(employeeId, request);

        // Assert
        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(n => n.NotificationId == 1);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenExists_ShouldDeleteAndSave()
    {
        // Arrange
        var notification = new Notification { NotificationId = 1, EmployeeId = 1, Title = "Welcome" };
        _notificationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notification);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _notificationRepositoryMock.Verify(r => r.Delete(notification), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
