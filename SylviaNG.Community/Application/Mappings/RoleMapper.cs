using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class RoleMapper
    {
        public static Role ToEntity(this RoleCreateRequest request)
        {
            return new Role
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Role entity, RoleUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RoleResponse ToResponse(this Role entity)
        {
            return new RoleResponse
            {
                RoleId = entity.RoleId,
                Name = entity.Name,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                IsActive = entity.IsActive
            };
        }
    }
}
