using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class NotificationPreferenceServiceTests
{
    private readonly Mock<INotificationPreferenceRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly NotificationPreferenceService _service;

    public NotificationPreferenceServiceTests()
    {
        _repositoryMock = new Mock<INotificationPreferenceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new NotificationPreferenceService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpsertAsync_WhenNoExistingPreference_ShouldCreateNew()
    {
        // Arrange
        var request = new NotificationPreferenceUpsertRequest { EmployeeId = 1, Category = "Announcements", InAppEnabled = true, EmailEnabled = false };
        _repositoryMock.Setup(r => r.GetAsync(1, "Announcements")).ReturnsAsync((NotificationPreference?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<NotificationPreference>()))
            .Callback<NotificationPreference>(p => p.PreferenceId = 5);

        // Act
        var result = await _service.UpsertAsync(request);

        // Assert
        result.Should().Be(5);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<NotificationPreference>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpsertAsync_WhenExistingPreference_ShouldUpdateExisting()
    {
        // Arrange
        var existing = new NotificationPreference { PreferenceId = 5, EmployeeId = 1, Category = "Announcements", InAppEnabled = false, EmailEnabled = false };
        var request = new NotificationPreferenceUpsertRequest { EmployeeId = 1, Category = "Announcements", InAppEnabled = true, EmailEnabled = true };
        _repositoryMock.Setup(r => r.GetAsync(1, "Announcements")).ReturnsAsync(existing);

        // Act
        var result = await _service.UpsertAsync(request);

        // Assert
        result.Should().Be(5);
        existing.InAppEnabled.Should().BeTrue();
        existing.EmailEnabled.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(existing), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<NotificationPreference>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByEmployeeAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var preferences = new List<NotificationPreference>
        {
            new() { PreferenceId = 1, EmployeeId = 1, Category = "Announcements" },
            new() { PreferenceId = 2, EmployeeId = 1, Category = "Teams" }
        };
        _repositoryMock.Setup(r => r.GetByEmployeeAsync(1)).ReturnsAsync(preferences);

        // Act
        var result = await _service.GetByEmployeeAsync(1);

        // Assert
        result.Should().HaveCount(2);
    }
}
