using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IGroupMemberRepository> _groupMemberRepositoryMock;
    private readonly Mock<IGroupJoinRequestRepository> _groupJoinRequestRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GroupService _service;

    public GroupServiceTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _groupMemberRepositoryMock = new Mock<IGroupMemberRepository>();
        _groupJoinRequestRepositoryMock = new Mock<IGroupJoinRequestRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new GroupService(
            _groupRepositoryMock.Object,
            _groupMemberRepositoryMock.Object,
            _groupJoinRequestRepositoryMock.Object,
            _postRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldSeedCallerAsCreator()
    {
        // Arrange
        var request = new GroupCreateRequest { Name = "Photography Club", Visibility = GroupVisibilityEnum.Public };
        _groupRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Group>()))
            .Callback<Group>(g => g.GroupId = 1);

        // Act
        var result = await _service.CreateAsync(request, callerEmployeeId: 5);

        // Assert
        result.Should().Be(1);
        _groupMemberRepositoryMock.Verify(r => r.AddAsync(It.Is<GroupMember>(
            m => m.GroupId == 1 && m.EmployeeId == 5 && m.Role == GroupMemberRoleEnum.Creator)), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsNotCreatorAndNotHr_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Group { GroupId = 1, Name = "Group" });
        _groupMemberRepositoryMock.Setup(r => r.GetActiveCreatorAsync(1))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 99, Role = GroupMemberRoleEnum.Creator });

        // Act
        var act = () => _service.DeleteAsync(1, callerEmployeeId: 5, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _groupRepositoryMock.Verify(r => r.Delete(It.IsAny<Group>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsHrOrAdmin_ShouldSucceedEvenIfNotCreator()
    {
        // Arrange
        var group = new Group { GroupId = 1, Name = "Group" };
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(group);

        // Act
        await _service.DeleteAsync(1, callerEmployeeId: 5, isHrOrAdmin: true);

        // Assert
        _groupRepositoryMock.Verify(r => r.Delete(group), Times.Once);
        _groupMemberRepositoryMock.Verify(r => r.GetActiveCreatorAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task JoinAsync_WhenGroupIsPrivate_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Private });

        // Act
        var act = () => _service.JoinAsync(1, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task JoinAsync_WhenAlreadyActiveMember_ShouldThrowDuplicateException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Public });
        _groupMemberRepositoryMock.Setup(r => r.GetAsync(1, 5))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 5, IsActive = true });

        // Act
        var act = () => _service.JoinAsync(1, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task RequestToJoinAsync_WhenGroupIsPublic_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Public });

        // Act
        var act = () => _service.RequestToJoinAsync(1, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RequestToJoinAsync_WhenAlreadyPending_ShouldThrowDuplicateException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Private });
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 5)).ReturnsAsync((GroupMember?)null);
        _groupJoinRequestRepositoryMock.Setup(r => r.GetPendingAsync(1, 5))
            .ReturnsAsync(new GroupJoinRequest { GroupId = 1, EmployeeId = 5, Status = GroupJoinRequestStatusEnum.Pending });

        // Act
        var act = () => _service.RequestToJoinAsync(1, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task ApproveJoinRequestAsync_ShouldActivateMembershipAndNotifyRequester()
    {
        // Arrange
        var request = new GroupJoinRequest { GroupJoinRequestId = 7, GroupId = 1, EmployeeId = 5, Status = GroupJoinRequestStatusEnum.Pending };
        _groupJoinRequestRepositoryMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 2))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 2, Role = GroupMemberRoleEnum.Creator });
        _groupMemberRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync((GroupMember?)null);
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Group { GroupId = 1, Name = "Group" });

        // Act
        await _service.ApproveJoinRequestAsync(7, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        request.Status.Should().Be(GroupJoinRequestStatusEnum.Approved);
        _groupMemberRepositoryMock.Verify(r => r.AddAsync(It.Is<GroupMember>(
            m => m.GroupId == 1 && m.EmployeeId == 5 && m.Role == GroupMemberRoleEnum.Member)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<Application.Features.Notifications.Models.NotificationCreateRequest>(
            r => r.EmployeeId == 5)), Times.Once);
    }

    [Fact]
    public async Task ApproveJoinRequestAsync_WhenCallerIsPlainMember_ShouldThrowForbiddenException()
    {
        // Arrange
        var request = new GroupJoinRequest { GroupJoinRequestId = 7, GroupId = 1, EmployeeId = 5, Status = GroupJoinRequestStatusEnum.Pending };
        _groupJoinRequestRepositoryMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 2))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 2, Role = GroupMemberRoleEnum.Member });

        // Act
        var act = () => _service.ApproveJoinRequestAsync(7, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RemoveMemberAsync_WhenGroupAdminRemovesCreatorWithoutHr_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 9))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 9, Role = GroupMemberRoleEnum.Creator });
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 2))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 2, Role = GroupMemberRoleEnum.GroupAdmin });

        // Act
        var act = () => _service.RemoveMemberAsync(1, employeeId: 9, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RemoveMemberAsync_WhenHrRemovesCreator_ShouldSucceed()
    {
        // Arrange
        var creator = new GroupMember { GroupId = 1, EmployeeId = 9, Role = GroupMemberRoleEnum.Creator, IsActive = true };
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 9)).ReturnsAsync(creator);
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Group { GroupId = 1, Name = "Group" });

        // Act
        await _service.RemoveMemberAsync(1, employeeId: 9, callerEmployeeId: 2, isHrOrAdmin: true);

        // Assert
        creator.IsActive.Should().BeFalse();
        _groupMemberRepositoryMock.Verify(r => r.Update(creator), Times.Once);
    }

    [Fact]
    public async Task LeaveAsync_WhenSoleCreator_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 5))
            .ReturnsAsync(new GroupMember { GroupId = 1, EmployeeId = 5, Role = GroupMemberRoleEnum.Creator });

        // Act
        var act = () => _service.LeaveAsync(1, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task ChangeMemberRoleAsync_OwnershipTransfer_ShouldDemotePreviousCreatorToGroupAdmin()
    {
        // Arrange
        var previousCreator = new GroupMember { GroupMemberId = 1, GroupId = 1, EmployeeId = 2, Role = GroupMemberRoleEnum.Creator };
        var newOwner = new GroupMember { GroupMemberId = 2, GroupId = 1, EmployeeId = 5, Role = GroupMemberRoleEnum.GroupAdmin };
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 5)).ReturnsAsync(newOwner);
        _groupMemberRepositoryMock.Setup(r => r.GetActiveCreatorAsync(1)).ReturnsAsync(previousCreator);
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Group { GroupId = 1, Name = "Group" });

        var request = new GroupMemberRoleChangeRequest { EmployeeId = 5, NewRole = GroupMemberRoleEnum.Creator };

        // Act
        await _service.ChangeMemberRoleAsync(1, request, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        previousCreator.Role.Should().Be(GroupMemberRoleEnum.GroupAdmin);
        newOwner.Role.Should().Be(GroupMemberRoleEnum.Creator);
    }

    [Fact]
    public async Task ChangeMemberRoleAsync_OwnershipTransfer_WhenCallerIsNotCreator_ShouldThrowForbiddenException()
    {
        // Arrange
        var newOwner = new GroupMember { GroupMemberId = 2, GroupId = 1, EmployeeId = 5, Role = GroupMemberRoleEnum.GroupAdmin };
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 5)).ReturnsAsync(newOwner);
        _groupMemberRepositoryMock.Setup(r => r.GetActiveCreatorAsync(1))
            .ReturnsAsync(new GroupMember { GroupMemberId = 1, GroupId = 1, EmployeeId = 99, Role = GroupMemberRoleEnum.Creator });

        var request = new GroupMemberRoleChangeRequest { EmployeeId = 5, NewRole = GroupMemberRoleEnum.Creator };

        // Act
        var act = () => _service.ChangeMemberRoleAsync(1, request, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetPostsAsync_WhenPrivateGroupAndCallerNotMember_ShouldThrowForbiddenException()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Private });
        _groupMemberRepositoryMock.Setup(r => r.GetActiveAsync(1, 5)).ReturnsAsync((GroupMember?)null);

        // Act
        var act = () => _service.GetPostsAsync(1, new PostFilterRequest(), callerEmployeeId: 5, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetPostsAsync_WhenPrivateGroupAndCallerIsHr_ShouldNotThrow()
    {
        // Arrange
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Group { GroupId = 1, Visibility = GroupVisibilityEnum.Private });
        _postRepositoryMock.Setup(r => r.GetGroupFeedPaginatedAsync(It.IsAny<PostFilterRequest>(), 1))
            .ReturnsAsync(new PagedResult<Post> { Data = new List<Post>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        var act = () => _service.GetPostsAsync(1, new PostFilterRequest(), callerEmployeeId: 5, isHrOrAdmin: true);

        // Assert
        await act.Should().NotThrowAsync();
        _groupMemberRepositoryMock.Verify(r => r.GetActiveAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task GetMyGroupsAsync_WhenEmployeeHasNoActiveMemberships_ShouldReturnEmptyListWithoutQueryingGroups()
    {
        // Arrange
        _groupMemberRepositoryMock.Setup(r => r.GetActiveGroupIdsByEmployeeIdAsync(5))
            .ReturnsAsync(new List<long>());

        // Act
        var result = await _service.GetMyGroupsAsync(5);

        // Assert
        result.Should().BeEmpty();
        _groupRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Group, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetMyGroupsAsync_ShouldReturnGroupsWithMemberCountsSortedByName()
    {
        // Arrange
        var groups = new List<Group>
        {
            new() { GroupId = 2, Name = "Rock Climbing", Visibility = GroupVisibilityEnum.Public },
            new() { GroupId = 1, Name = "Book Club", Visibility = GroupVisibilityEnum.Private },
        };
        _groupMemberRepositoryMock.Setup(r => r.GetActiveGroupIdsByEmployeeIdAsync(5))
            .ReturnsAsync(new List<long> { 1, 2 });
        _groupRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Group, bool>>>()))
            .ReturnsAsync(groups);
        _groupMemberRepositoryMock.Setup(r => r.GetActiveMemberCountsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new Dictionary<long, int> { { 1, 4 }, { 2, 9 } });

        // Act
        var result = await _service.GetMyGroupsAsync(5);

        // Assert
        result.Should().HaveCount(2);
        result[0].GroupId.Should().Be(1);
        result[0].MemberCount.Should().Be(4);
        result[1].GroupId.Should().Be(2);
        result[1].MemberCount.Should().Be(9);
    }
}
