using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class BadgeServiceTests
{
    private readonly Mock<IBadgeRepository> _badgeRepositoryMock;
    private readonly Mock<IEmployeeBadgeRepository> _employeeBadgeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BadgeService _service;

    public BadgeServiceTests()
    {
        _badgeRepositoryMock = new Mock<IBadgeRepository>();
        _employeeBadgeRepositoryMock = new Mock<IEmployeeBadgeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new BadgeService(_badgeRepositoryMock.Object, _employeeBadgeRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new BadgeCreateRequest { Name = "Rockstar" };

        _badgeRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _badgeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Badge>()))
            .Callback<Badge>(b => b.BadgeId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new BadgeCreateRequest { Name = "Rockstar" };
        _badgeRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task AwardToEmployeeAsync_WhenBadgeNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _badgeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Badge?)null);

        // Act
        var act = () => _service.AwardToEmployeeAsync(5, new EmployeeBadgeAwardRequest { BadgeId = 1 });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AwardToEmployeeAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        _badgeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Badge { BadgeId = 1, Name = "Rockstar" });
        _employeeBadgeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EmployeeBadge>()))
            .Callback<EmployeeBadge>(eb => eb.EmployeeBadgeId = 10);

        // Act
        var result = await _service.AwardToEmployeeAsync(5, new EmployeeBadgeAwardRequest { BadgeId = 1 });

        // Assert
        result.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
