using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class BranchMapper
    {
        public static Branch ToEntity(this BranchCreateRequest request)
        {
            return new Branch
            {
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Branch entity, BranchUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Address != null) entity.Address = request.Address;
            if (request.City != null) entity.City = request.City;
            if (request.Country != null) entity.Country = request.Country;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static BranchResponse ToResponse(this Branch entity)
        {
            return new BranchResponse
            {
                BranchId = entity.BranchId,
                Name = entity.Name,
                Address = entity.Address,
                City = entity.City,
                Country = entity.Country,
                CreatedBy = entity.CreatedBy,
                IsActive = entity.IsActive
            };
        }
    }
}
