using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class BadgeMapper
    {
        public static Badge ToEntity(this BadgeCreateRequest request)
        {
            return new Badge
            {
                Name = request.Name,
                Icon = request.Icon,
                Description = request.Description
            };
        }

        public static BadgeResponse ToResponse(this Badge entity)
        {
            return new BadgeResponse
            {
                BadgeId = entity.BadgeId,
                Name = entity.Name,
                Icon = entity.Icon,
                Description = entity.Description
            };
        }

        public static EmployeeBadge ToEntity(this EmployeeBadgeAwardRequest request, long employeeId)
        {
            return new EmployeeBadge
            {
                EmployeeId = employeeId,
                BadgeId = request.BadgeId,
                AwardedDate = request.AwardedDate ?? DateTime.UtcNow
            };
        }

        public static EmployeeBadgeResponse ToResponse(this EmployeeBadge entity)
        {
            return new EmployeeBadgeResponse
            {
                EmployeeBadgeId = entity.EmployeeBadgeId,
                EmployeeId = entity.EmployeeId,
                BadgeId = entity.BadgeId,
                AwardedDate = entity.AwardedDate
            };
        }
    }
}
