using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class TeamServiceTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TeamService _service;

    public TeamServiceTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new TeamService(_teamRepositoryMock.Object, _teamMemberRepositoryMock.Object, _notificationServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new TeamCreateRequest { Name = "Engineering" };

        _teamRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _teamRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Team>()))
            .Callback<Team>(t => t.TeamId = 1);

        // Act
        var result = await _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new TeamCreateRequest { Name = "Engineering" };
        _teamRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>().WithMessage("*Engineering*");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Team?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddMemberAsync_WhenAlreadyMember_ShouldThrowDuplicateException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering" });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(true);

        // Act
        var act = () => _service.AddMemberAsync(1, new TeamMemberAddRequest { EmployeeId = 5 }, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveMemberAsync_WhenMemberExists_ShouldSetIsActiveFalse()
    {
        // Arrange
        var member = new TeamMember { TeamMemberId = 10, TeamId = 1, EmployeeId = 5, IsActive = true };
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering" });
        _teamMemberRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync(member);

        // Act
        await _service.RemoveMemberAsync(1, 5, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        member.IsActive.Should().BeFalse();
        _teamMemberRepositoryMock.Verify(r => r.Update(member), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithSupervisor_ShouldNotifyTheSupervisor()
    {
        // Arrange
        var request = new TeamCreateRequest { Name = "Engineering", SupervisorId = 42 };
        _teamRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _teamRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Team>())).Callback<Team>(t => t.TeamId = 1);

        // Act
        await _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 42 && r.Category == "Team" && r.RelatedEntityId == 1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithoutSupervisor_ShouldNotNotifyAnyone()
    {
        // Arrange
        var request = new TeamCreateRequest { Name = "Engineering" };
        _teamRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _teamRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Team>())).Callback<Team>(t => t.TeamId = 1);

        // Act
        await _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddMemberAsync_ShouldNotifyTheNewMember()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering" });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 5)).ReturnsAsync(false);

        // Act
        await _service.AddMemberAsync(1, new TeamMemberAddRequest { EmployeeId = 5 }, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 5 && r.Category == "Team")), Times.Once);
    }
}
