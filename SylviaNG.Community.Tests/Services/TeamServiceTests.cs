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
using SylviaNG.Community.SharedKernel.Pagination;

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
        var act = () => _service.GetByIdAsync(1, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenCallerIsNeitherSupervisorMemberNorHrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(false);

        // Act
        var act = () => _service.GetByIdAsync(1, callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenCallerIsSupervisor_ShouldSucceed()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 7 });

        // Act
        var result = await _service.GetByIdAsync(1, callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        result.TeamId.Should().Be(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenCallerIsActiveMember_ShouldSucceed()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(true);

        // Act
        var result = await _service.GetByIdAsync(1, callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        result.TeamId.Should().Be(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenCallerIsHrOrAdmin_ShouldSucceedRegardlessOfMembership()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });

        // Act
        var result = await _service.GetByIdAsync(1, callerEmployeeId: 99, isHrOrAdmin: true);

        // Assert
        result.TeamId.Should().Be(1);
        _teamMemberRepositoryMock.Verify(r => r.ExistsAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_WhenHrOrAdmin_ShouldCallRepositoryUnscoped()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>(), null, null))
            .ReturnsAsync(new PagedResult<Team> { Data = new List<Team>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetPaginatedAsync(new PagedRequest(), callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        _teamRepositoryMock.Verify(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>(), null, null), Times.Once);
        _teamMemberRepositoryMock.Verify(r => r.GetTeamIdsByEmployeeIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_WhenRegularEmployee_ShouldScopeToTheirTeams()
    {
        // Arrange
        _teamMemberRepositoryMock.Setup(r => r.GetTeamIdsByEmployeeIdAsync(7)).ReturnsAsync(new List<long> { 2, 3 });
        _teamRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>(), 7, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new PagedResult<Team> { Data = new List<Team>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetPaginatedAsync(new PagedRequest(), callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        _teamRepositoryMock.Verify(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>(), 7, It.Is<IEnumerable<long>>(ids => ids.SequenceEqual(new long[] { 2, 3 }))), Times.Once);
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

    [Fact]
    public async System.Threading.Tasks.Task GetMembersAsync_WhenCallerIsNeitherSupervisorMemberNorHrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(false);

        // Act
        var act = () => _service.GetMembersAsync(1, callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetMembersAsync_WhenCallerIsActiveMember_ShouldReturnMembers()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(true);
        _teamMemberRepositoryMock.Setup(r => r.GetByTeamIdAsync(1)).ReturnsAsync(new List<TeamMember> { new() { TeamMemberId = 1, TeamId = 1, EmployeeId = 7, IsActive = true } });

        // Act
        var result = await _service.GetMembersAsync(1, callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        result.Should().ContainSingle(m => m.EmployeeId == 7);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetMembersAsync_WhenCallerIsHrOrAdmin_ShouldSucceedRegardlessOfMembership()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, Name = "Engineering", SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.GetByTeamIdAsync(1)).ReturnsAsync(new List<TeamMember>());

        // Act
        await _service.GetMembersAsync(1, callerEmployeeId: 99, isHrOrAdmin: true);

        // Assert
        _teamMemberRepositoryMock.Verify(r => r.ExistsAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }
}
