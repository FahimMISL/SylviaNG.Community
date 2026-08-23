using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupJoinRequestRepository _groupJoinRequestRepository;
        private readonly IPostRepository _postRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public GroupService(
            IGroupRepository groupRepository,
            IGroupMemberRepository groupMemberRepository,
            IGroupJoinRequestRepository groupJoinRequestRepository,
            IPostRepository postRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
            _postRepository = postRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        private async Task<string> GetEmployeeNameAsync(long employeeId) =>
            (await _employeeRepository.GetByIdAsync(employeeId))?.EmployeeName ?? "Someone";

        public async Task<long> CreateAsync(GroupCreateRequest request, long callerEmployeeId)
        {
            var entity = request.ToEntity();
            await _groupRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Creator only - no other members are auto-added (US-3.16).
            var creatorMembership = GroupMapper.ToNewMember(entity.GroupId, callerEmployeeId, GroupMemberRoleEnum.Creator);
            await _groupMemberRepository.AddAsync(creatorMembership);
            await _unitOfWork.SaveChangesAsync();

            return entity.GroupId;
        }

        public async Task UpdateAsync(long groupId, GroupUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            await EnsureManagerAsync(groupId, callerEmployeeId, isHrOrAdmin);

            var wasPrivate = entity.Visibility == GroupVisibilityEnum.Private;
            entity.ApplyUpdate(request);
            _groupRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            // Private -> Public: auto-approve every pending request (US-3.26 AC3).
            // Public -> Private needs no extra work here - global-feed exclusion of this
            // group's posts is computed live from Group.Visibility at feed-query time.
            if (wasPrivate && entity.Visibility == GroupVisibilityEnum.Public)
            {
                await AutoApprovePendingRequestsAsync(entity, callerEmployeeId);
            }
        }

        public async Task DeleteAsync(long groupId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            if (!isHrOrAdmin)
            {
                var creator = await _groupMemberRepository.GetActiveCreatorAsync(groupId);
                if (creator == null || creator.EmployeeId != callerEmployeeId)
                    throw new ForbiddenException("Only the group's creator can delete it.");
            }

            // Cascade delete (configured on the FKs) removes memberships, join requests,
            // and the group's posts (which in turn cascade to comments/reactions/attachments).
            _groupRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<GroupResponse> GetByIdAsync(long groupId)
        {
            var entity = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            var counts = await _groupMemberRepository.GetActiveMemberCountsAsync(new[] { groupId });
            return entity.ToResponse(counts.GetValueOrDefault(groupId));
        }

        public async Task<PagedResult<GroupResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _groupRepository.GetPaginatedAsync(request);
            var groupIds = pagedResult.Data.Select(g => g.GroupId).ToList();
            var counts = await _groupMemberRepository.GetActiveMemberCountsAsync(groupIds);

            return new PagedResult<GroupResponse>
            {
                Data = pagedResult.Data.Select(g => g.ToResponse(counts.GetValueOrDefault(g.GroupId))).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<GroupResponse>> GetMyGroupsAsync(long employeeId)
        {
            var groupIds = await _groupMemberRepository.GetActiveGroupIdsByEmployeeIdAsync(employeeId);
            if (groupIds.Count == 0)
                return new List<GroupResponse>();

            var groups = await _groupRepository.FindAsync(g => groupIds.Contains(g.GroupId));
            var counts = await _groupMemberRepository.GetActiveMemberCountsAsync(groupIds);

            return groups
                .OrderBy(g => g.Name)
                .Select(g => g.ToResponse(counts.GetValueOrDefault(g.GroupId)))
                .ToList();
        }

        public async Task JoinAsync(long groupId, long callerEmployeeId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            if (group.Visibility != GroupVisibilityEnum.Public)
                throw new ForbiddenException("This group requires an approved join request - use Request to Join instead.");

            await AddOrReactivateMemberAsync(groupId, callerEmployeeId, GroupMemberRoleEnum.Member, blockIfAlreadyActive: true);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<long> RequestToJoinAsync(long groupId, long callerEmployeeId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            if (group.Visibility != GroupVisibilityEnum.Private)
                throw new ForbiddenException("This group is public - use Join instead of requesting to join.");

            var activeMembership = await _groupMemberRepository.GetActiveAsync(groupId, callerEmployeeId);
            if (activeMembership != null)
                throw new DuplicateException("GroupMember", "EmployeeId", callerEmployeeId.ToString());

            var existingPending = await _groupJoinRequestRepository.GetPendingAsync(groupId, callerEmployeeId);
            if (existingPending != null)
                throw new DuplicateException("GroupJoinRequest", "EmployeeId", callerEmployeeId.ToString());

            var request = new GroupJoinRequest
            {
                GroupId = groupId,
                EmployeeId = callerEmployeeId,
                Status = GroupJoinRequestStatusEnum.Pending
            };
            await _groupJoinRequestRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var approvers = await _groupMemberRepository.GetActiveByRolesAsync(groupId, GroupMemberRoleEnum.Creator, GroupMemberRoleEnum.GroupAdmin);
            var requesterName = await GetEmployeeNameAsync(callerEmployeeId);
            foreach (var approver in approvers)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = approver.EmployeeId,
                    Title = $"{requesterName} requested to join \"{group.Name}\"",
                    Category = "GroupJoinRequest",
                    RelatedEntityType = "Group",
                    RelatedEntityId = groupId
                });
            }

            return request.GroupJoinRequestId;
        }

        public async Task ApproveJoinRequestAsync(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var request = await _groupJoinRequestRepository.GetByIdAsync(groupJoinRequestId)
                ?? throw new NotFoundException("GroupJoinRequest", groupJoinRequestId);

            if (request.Status != GroupJoinRequestStatusEnum.Pending)
                throw new ForbiddenException("This request has already been resolved.");

            await EnsureManagerAsync(request.GroupId, callerEmployeeId, isHrOrAdmin);

            request.Status = GroupJoinRequestStatusEnum.Approved;
            request.ResolvedByEmployeeId = callerEmployeeId;
            request.ResolvedAt = DateTime.UtcNow;
            _groupJoinRequestRepository.Update(request);

            await AddOrReactivateMemberAsync(request.GroupId, request.EmployeeId, GroupMemberRoleEnum.Member, blockIfAlreadyActive: false);
            await _unitOfWork.SaveChangesAsync();

            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            var approverName = await GetEmployeeNameAsync(callerEmployeeId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = request.EmployeeId,
                Title = $"{approverName} approved your request to join \"{group?.Name}\"",
                Category = "GroupJoinRequest",
                RelatedEntityType = "Group",
                RelatedEntityId = request.GroupId
            });
        }

        public async Task RejectJoinRequestAsync(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var request = await _groupJoinRequestRepository.GetByIdAsync(groupJoinRequestId)
                ?? throw new NotFoundException("GroupJoinRequest", groupJoinRequestId);

            if (request.Status != GroupJoinRequestStatusEnum.Pending)
                throw new ForbiddenException("This request has already been resolved.");

            await EnsureManagerAsync(request.GroupId, callerEmployeeId, isHrOrAdmin);

            request.Status = GroupJoinRequestStatusEnum.Rejected;
            request.ResolvedByEmployeeId = callerEmployeeId;
            request.ResolvedAt = DateTime.UtcNow;
            _groupJoinRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            var approverName = await GetEmployeeNameAsync(callerEmployeeId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = request.EmployeeId,
                Title = $"{approverName} declined your request to join \"{group?.Name}\"",
                Category = "GroupJoinRequest",
                RelatedEntityType = "Group",
                RelatedEntityId = request.GroupId
            });
        }

        public async Task<List<GroupJoinRequestResponse>> GetPendingJoinRequestsAsync(long groupId, long callerEmployeeId, bool isHrOrAdmin)
        {
            _ = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            await EnsureManagerAsync(groupId, callerEmployeeId, isHrOrAdmin);

            var requests = await _groupJoinRequestRepository.GetPendingByGroupIdAsync(groupId);
            return requests.Select(r => r.ToResponse()).ToList();
        }

        public async Task<long> AddMemberAsync(long groupId, GroupMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            _ = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            await EnsureManagerAsync(groupId, callerEmployeeId, isHrOrAdmin);

            var memberId = await AddOrReactivateMemberAsync(groupId, request.EmployeeId, GroupMemberRoleEnum.Member, blockIfAlreadyActive: true);
            await _unitOfWork.SaveChangesAsync();

            var group = await _groupRepository.GetByIdAsync(groupId);
            var adminName = await GetEmployeeNameAsync(callerEmployeeId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = request.EmployeeId,
                Title = $"{adminName} added you to \"{group?.Name}\"",
                Category = "GroupMembership",
                RelatedEntityType = "Group",
                RelatedEntityId = groupId
            });

            return memberId;
        }

        public async Task RemoveMemberAsync(long groupId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var member = await _groupMemberRepository.GetActiveAsync(groupId, employeeId)
                ?? throw new NotFoundException("GroupMember", employeeId);

            if (!isHrOrAdmin)
            {
                await EnsureManagerAsync(groupId, callerEmployeeId, isHrOrAdmin);

                if (member.Role == GroupMemberRoleEnum.Creator)
                    throw new ForbiddenException("The group's creator cannot be removed - transfer ownership or delete the group instead.");
            }

            // Demote+remove a Group Admin in one action.
            member.Role = GroupMemberRoleEnum.Member;
            member.IsActive = false;
            _groupMemberRepository.Update(member);
            await _unitOfWork.SaveChangesAsync();

            var group = await _groupRepository.GetByIdAsync(groupId);
            var actorName = await GetEmployeeNameAsync(callerEmployeeId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = employeeId,
                Title = $"{actorName} removed you from \"{group?.Name}\"",
                Category = "GroupMembership",
                RelatedEntityType = "Group",
                RelatedEntityId = groupId
            });
        }

        public async Task ChangeMemberRoleAsync(long groupId, GroupMemberRoleChangeRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var member = await _groupMemberRepository.GetActiveAsync(groupId, request.EmployeeId)
                ?? throw new NotFoundException("GroupMember", request.EmployeeId);

            if (request.NewRole == GroupMemberRoleEnum.Creator)
            {
                // Ownership transfer - Creator (or HR/Admin) only (US-3.25 AC3).
                if (!isHrOrAdmin)
                {
                    var currentCreator = await _groupMemberRepository.GetActiveCreatorAsync(groupId);
                    if (currentCreator == null || currentCreator.EmployeeId != callerEmployeeId)
                        throw new ForbiddenException("Only the group's current creator can transfer ownership.");
                }

                var previousCreator = await _groupMemberRepository.GetActiveCreatorAsync(groupId);
                if (previousCreator != null && previousCreator.GroupMemberId != member.GroupMemberId)
                {
                    previousCreator.Role = GroupMemberRoleEnum.GroupAdmin;
                    _groupMemberRepository.Update(previousCreator);
                }
            }
            else
            {
                await EnsureManagerAsync(groupId, callerEmployeeId, isHrOrAdmin);
            }

            member.Role = request.NewRole;
            _groupMemberRepository.Update(member);
            await _unitOfWork.SaveChangesAsync();

            var group = await _groupRepository.GetByIdAsync(groupId);
            var actorName = await GetEmployeeNameAsync(callerEmployeeId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = request.EmployeeId,
                Title = $"{actorName} changed your role in \"{group?.Name}\" to {request.NewRole}",
                Category = "GroupMembership",
                RelatedEntityType = "Group",
                RelatedEntityId = groupId
            });
        }

        public async Task LeaveAsync(long groupId, long callerEmployeeId)
        {
            var member = await _groupMemberRepository.GetActiveAsync(groupId, callerEmployeeId)
                ?? throw new NotFoundException("GroupMember", callerEmployeeId);

            if (member.Role == GroupMemberRoleEnum.Creator)
                throw new ForbiddenException("Transfer ownership to another admin or delete the group before leaving.");

            member.IsActive = false;
            _groupMemberRepository.Update(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<GroupMemberResponse>> GetMembersAsync(long groupId)
        {
            var members = await _groupMemberRepository.GetActiveByGroupIdAsync(groupId);
            return members.Select(m => m.ToResponse()).ToList();
        }

        public async Task<PagedResult<PostResponse>> GetPostsAsync(long groupId, PostFilterRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var group = await _groupRepository.GetByIdAsync(groupId)
                ?? throw new NotFoundException("Group", groupId);

            if (group.Visibility == GroupVisibilityEnum.Private && !isHrOrAdmin)
            {
                var membership = await _groupMemberRepository.GetActiveAsync(groupId, callerEmployeeId);
                if (membership == null)
                    throw new ForbiddenException("You must be a member of this private group to view its posts.");
            }

            var pagedResult = await _postRepository.GetGroupFeedPaginatedAsync(request, groupId);

            return new PagedResult<PostResponse>
            {
                Data = pagedResult.Data.Select(p => p.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private async Task EnsureManagerAsync(long groupId, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (isHrOrAdmin)
                return;

            var caller = await _groupMemberRepository.GetActiveAsync(groupId, callerEmployeeId);
            if (caller == null || (caller.Role != GroupMemberRoleEnum.Creator && caller.Role != GroupMemberRoleEnum.GroupAdmin))
                throw new ForbiddenException("Only the group's creator or a group admin can do this.");
        }

        /// <summary>Inserts a fresh membership, or reactivates a previously-removed one, for (groupId, employeeId).</summary>
        private async Task<long> AddOrReactivateMemberAsync(long groupId, long employeeId, GroupMemberRoleEnum role, bool blockIfAlreadyActive)
        {
            var existing = await _groupMemberRepository.GetAsync(groupId, employeeId);
            if (existing != null)
            {
                if (existing.IsActive)
                {
                    if (blockIfAlreadyActive)
                        throw new DuplicateException("GroupMember", "EmployeeId", employeeId.ToString());

                    return existing.GroupMemberId;
                }

                existing.IsActive = true;
                existing.Role = role;
                existing.JoinedDate = DateTime.UtcNow;
                _groupMemberRepository.Update(existing);
                return existing.GroupMemberId;
            }

            var member = GroupMapper.ToNewMember(groupId, employeeId, role);
            await _groupMemberRepository.AddAsync(member);
            return member.GroupMemberId;
        }

        private async Task AutoApprovePendingRequestsAsync(Group group, long resolvedByEmployeeId)
        {
            var pendingRequests = await _groupJoinRequestRepository.GetPendingByGroupIdAsync(group.GroupId);
            var resolverName = await GetEmployeeNameAsync(resolvedByEmployeeId);
            foreach (var pending in pendingRequests)
            {
                pending.Status = GroupJoinRequestStatusEnum.Approved;
                pending.ResolvedByEmployeeId = resolvedByEmployeeId;
                pending.ResolvedAt = DateTime.UtcNow;
                _groupJoinRequestRepository.Update(pending);

                await AddOrReactivateMemberAsync(group.GroupId, pending.EmployeeId, GroupMemberRoleEnum.Member, blockIfAlreadyActive: false);

                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = pending.EmployeeId,
                    Title = $"{resolverName} approved your request to join \"{group.Name}\"",
                    Category = "GroupJoinRequest",
                    RelatedEntityType = "Group",
                    RelatedEntityId = group.GroupId
                });
            }

            if (pendingRequests.Count > 0)
                await _unitOfWork.SaveChangesAsync();
        }
    }
}
