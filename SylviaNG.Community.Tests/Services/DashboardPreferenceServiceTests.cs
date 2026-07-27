using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class DashboardPreferenceServiceTests
{
    private readonly Mock<IDashboardPreferenceRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DashboardPreferenceService _service;

    public DashboardPreferenceServiceTests()
    {
        _repositoryMock = new Mock<IDashboardPreferenceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new DashboardPreferenceService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpsertAsync_WhenNoExistingPreference_ShouldCreateNew()
    {
        // Arrange
        var request = new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "TeamRoster", DisplayOrder = 1, IsVisible = true };
        _repositoryMock.Setup(r => r.GetAsync(1, "TeamRoster")).ReturnsAsync((DashboardPreference?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<DashboardPreference>()))
            .Callback<DashboardPreference>(p => p.PreferenceId = 7);

        // Act
        var result = await _service.UpsertAsync(request);

        // Assert
        result.Should().Be(7);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpsertAsync_WhenExistingPreference_ShouldUpdateExisting()
    {
        // Arrange
        var existing = new DashboardPreference { PreferenceId = 7, EmployeeId = 1, WidgetName = "TeamRoster", DisplayOrder = 1, IsVisible = true };
        var request = new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "TeamRoster", DisplayOrder = 2, IsVisible = false };
        _repositoryMock.Setup(r => r.GetAsync(1, "TeamRoster")).ReturnsAsync(existing);

        // Act
        var result = await _service.UpsertAsync(request);

        // Assert
        result.Should().Be(7);
        existing.DisplayOrder.Should().Be(2);
        existing.IsVisible.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DashboardPreference?)null);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByEmployeeAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var preferences = new List<DashboardPreference>
        {
            new() { PreferenceId = 1, EmployeeId = 1, WidgetName = "TeamRoster" }
        };
        _repositoryMock.Setup(r => r.GetByEmployeeAsync(1)).ReturnsAsync(preferences);

        // Act
        var result = await _service.GetByEmployeeAsync(1);

        // Assert
        result.Should().ContainSingle(p => p.WidgetName == "TeamRoster");
    }
}
