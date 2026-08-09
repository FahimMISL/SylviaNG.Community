using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class TeamMapper
    {
        public static Team ToEntity(this TeamCreateRequest request)
        {
            return new Team
            {
                Name = request.Name,
                Description = request.Description,
                SupervisorId = request.SupervisorId,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Team entity, TeamUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.SupervisorId.HasValue) entity.SupervisorId = request.SupervisorId;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static TeamResponse ToResponse(this Team entity)
        {
            return new TeamResponse
            {
                TeamId = entity.TeamId,
                Name = entity.Name,
                Description = entity.Description,
                SupervisorId = entity.SupervisorId,
                CreatedBy = entity.CreatedBy,
                IsActive = entity.IsActive
            };
        }

        public static TeamMember ToEntity(this TeamMemberAddRequest request, long teamId)
        {
            return new TeamMember
            {
                TeamId = teamId,
                EmployeeId = request.EmployeeId,
                JoinedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static TeamMemberResponse ToResponse(this TeamMember entity)
        {
            return new TeamMemberResponse
            {
                TeamMemberId = entity.TeamMemberId,
                TeamId = entity.TeamId,
                EmployeeId = entity.EmployeeId,
                JoinedDate = entity.JoinedDate,
                IsActive = entity.IsActive
            };
        }
    }
}
