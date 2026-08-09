using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class DepartmentMapper
    {
        public static Department ToEntity(this DepartmentCreateRequest request)
        {
            return new Department
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Department entity, DepartmentUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Code != null) entity.Code = request.Code;
            if (request.Description != null) entity.Description = request.Description;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DepartmentResponse ToResponse(this Department entity)
        {
            return new DepartmentResponse
            {
                DepartmentId = entity.DepartmentId,
                Name = entity.Name,
                Code = entity.Code,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                IsActive = entity.IsActive
            };
        }
    }
}
