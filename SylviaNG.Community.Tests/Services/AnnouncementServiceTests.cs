using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class AnnouncementServiceTests
{
    private readonly Mock<IAnnouncementRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AnnouncementService _service;

    public AnnouncementServiceTests()
    {
        _repositoryMock = new Mock<IAnnouncementRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new AnnouncementService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnAnnouncementId()
    {
        // Arrange
        var request = new AnnouncementCreateRequest
        {
            Title = "Town Hall Meeting",
            SiteId = 1,
            DepartmentId = 1,
            AnnouncementType = AnnouncementTypeEnum.Event
        };

        _repositoryMock.Setup(r => r.ExistsByTitleAndSiteIdAsync(request.Title, request.SiteId, null))
            .ReturnsAsync(false);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Announcement>()))
            .Callback<Announcement>(a => a.AnnouncementId = 1);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Announcement>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateTitle_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new AnnouncementCreateRequest
        {
            Title = "Existing Announcement",
            SiteId = 1
        };

        _repositoryMock.Setup(r => r.ExistsByTitleAndSiteIdAsync(request.Title, request.SiteId, null))
            .ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>()
            .WithMessage("*Existing Announcement*");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteAndSave()
    {
        // Arrange
        var entity = new Announcement { AnnouncementId = 1, Title = "Test" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Announcement?)null);

        // Act
        var act = () => _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnMappedResponse()
    {
        // Arrange
        var entity = new Announcement
        {
            AnnouncementId = 1,
            Title = "Town Hall Meeting",
            SiteId = 1,
            IsActive = true
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.AnnouncementId.Should().Be(1);
        result.Title.Should().Be("Town Hall Meeting");
    }
}