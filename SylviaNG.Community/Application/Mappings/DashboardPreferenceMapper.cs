using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class DashboardPreferenceMapper
    {
        public static DashboardPreference ToEntity(this DashboardPreferenceUpsertRequest request)
        {
            return new DashboardPreference
            {
                EmployeeId = request.EmployeeId,
                WidgetName = request.WidgetName,
                DisplayOrder = request.DisplayOrder,
                IsVisible = request.IsVisible,
                LastModified = DateTime.UtcNow
            };
        }

        public static void ApplyUpdate(this DashboardPreference entity, DashboardPreferenceUpsertRequest request)
        {
            entity.DisplayOrder = request.DisplayOrder;
            entity.IsVisible = request.IsVisible;
            entity.LastModified = DateTime.UtcNow;
        }

        public static DashboardPreferenceResponse ToResponse(this DashboardPreference entity)
        {
            return new DashboardPreferenceResponse
            {
                PreferenceId = entity.PreferenceId,
                EmployeeId = entity.EmployeeId,
                WidgetName = entity.WidgetName,
                DisplayOrder = entity.DisplayOrder,
                IsVisible = entity.IsVisible,
                LastModified = entity.LastModified
            };
        }
    }
}
