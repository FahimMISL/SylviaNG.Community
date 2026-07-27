using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class AuditLogMapper
    {
        public static AuditLog ToEntity(this AuditLogCreateRequest request)
        {
            return new AuditLog
            {
                TableName = request.TableName,
                RecordId = request.RecordId,
                Action = request.Action,
                OldValue = request.OldValue,
                NewValue = request.NewValue,
                PerformedBy = request.PerformedBy
            };
        }

        public static AuditLogResponse ToResponse(this AuditLog entity)
        {
            return new AuditLogResponse
            {
                AuditId = entity.AuditId,
                TableName = entity.TableName,
                RecordId = entity.RecordId,
                Action = entity.Action,
                OldValue = entity.OldValue,
                NewValue = entity.NewValue,
                PerformedBy = entity.PerformedBy,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
