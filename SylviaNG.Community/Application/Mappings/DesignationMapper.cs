using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class DesignationMapper
    {
        public static Designation ToEntity(this DesignationCreateRequest request)
        {
            return new Designation
            {
                Name = request.Name,
                Grade = request.Grade,
                Description = request.Description,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Designation entity, DesignationUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Grade != null) entity.Grade = request.Grade;
            if (request.Description != null) entity.Description = request.Description;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DesignationResponse ToResponse(this Designation entity)
        {
            return new DesignationResponse
            {
                DesignationId = entity.DesignationId,
                Name = entity.Name,
                Grade = entity.Grade,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                IsActive = entity.IsActive
            };
        }
    }
}
