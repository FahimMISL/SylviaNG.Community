using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class NotificationMapper
    {
        public static Notification ToEntity(this NotificationCreateRequest request)
        {
            return new Notification
            {
                EmployeeId = request.EmployeeId,
                Title = request.Title,
                Message = request.Message,
                Category = request.Category,
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                IsRead = false
            };
        }

        public static NotificationResponse ToResponse(this Notification entity)
        {
            return new NotificationResponse
            {
                NotificationId = entity.NotificationId,
                EmployeeId = entity.EmployeeId,
                Title = entity.Title,
                Message = entity.Message,
                Category = entity.Category,
                RelatedEntityType = entity.RelatedEntityType,
                RelatedEntityId = entity.RelatedEntityId,
                IsRead = entity.IsRead,
                ReadAt = entity.ReadAt,
                CreatedAt = entity.CreatedAt
            };
        }

        public static NotificationPreference ToEntity(this NotificationPreferenceUpsertRequest request)
        {
            return new NotificationPreference
            {
                EmployeeId = request.EmployeeId,
                Category = request.Category,
                InAppEnabled = request.InAppEnabled,
                EmailEnabled = request.EmailEnabled
            };
        }

        public static void ApplyUpdate(this NotificationPreference entity, NotificationPreferenceUpsertRequest request)
        {
            entity.InAppEnabled = request.InAppEnabled;
            entity.EmailEnabled = request.EmailEnabled;
        }

        public static NotificationPreferenceResponse ToResponse(this NotificationPreference entity)
        {
            return new NotificationPreferenceResponse
            {
                PreferenceId = entity.PreferenceId,
                EmployeeId = entity.EmployeeId,
                Category = entity.Category,
                InAppEnabled = entity.InAppEnabled,
                EmailEnabled = entity.EmailEnabled
            };
        }
    }
}
