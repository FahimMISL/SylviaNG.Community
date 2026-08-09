using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Mappings
{
    public static class GroupMapper
    {
        public static Group ToEntity(this GroupCreateRequest request)
        {
            return new Group
            {
                Name = request.Name,
                Description = request.Description,
                Visibility = request.Visibility
            };
        }

        public static void ApplyUpdate(this Group entity, GroupUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.Visibility.HasValue) entity.Visibility = request.Visibility.Value;
        }

        public static GroupResponse ToResponse(this Group entity, int memberCount)
        {
            return new GroupResponse
            {
                GroupId = entity.GroupId,
                Name = entity.Name,
                Description = entity.Description,
                Visibility = entity.Visibility,
                MemberCount = memberCount,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt
            };
        }

        public static GroupMember ToNewMember(long groupId, long employeeId, GroupMemberRoleEnum role)
        {
            return new GroupMember
            {
                GroupId = groupId,
                EmployeeId = employeeId,
                Role = role,
                JoinedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static GroupMemberResponse ToResponse(this GroupMember entity)
        {
            return new GroupMemberResponse
            {
                GroupMemberId = entity.GroupMemberId,
                GroupId = entity.GroupId,
                EmployeeId = entity.EmployeeId,
                Role = entity.Role,
                JoinedDate = entity.JoinedDate,
                IsActive = entity.IsActive
            };
        }

        public static GroupJoinRequestResponse ToResponse(this GroupJoinRequest entity)
        {
            return new GroupJoinRequestResponse
            {
                GroupJoinRequestId = entity.GroupJoinRequestId,
                GroupId = entity.GroupId,
                EmployeeId = entity.EmployeeId,
                Status = entity.Status,
                ResolvedByEmployeeId = entity.ResolvedByEmployeeId,
                ResolvedAt = entity.ResolvedAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
