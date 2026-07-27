using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class ActivityLogMapper
    {
        public static ActivityLog ToEntity(this ActivityLogCreateRequest request)
        {
            return new ActivityLog
            {
                EmployeeId = request.EmployeeId,
                Module = request.Module,
                Action = request.Action,
                EntityType = request.EntityType,
                EntityId = request.EntityId
            };
        }

        public static ActivityLogResponse ToResponse(this ActivityLog entity)
        {
            return new ActivityLogResponse
            {
                ActivityId = entity.ActivityId,
                EmployeeId = entity.EmployeeId,
                Module = entity.Module,
                Action = entity.Action,
                EntityType = entity.EntityType,
                EntityId = entity.EntityId,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
