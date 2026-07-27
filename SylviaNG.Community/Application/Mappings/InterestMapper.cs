using SylviaNG.Community.Application.Features.EmployeeInterests.Models;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class InterestMapper
    {
        public static Interest ToEntity(this InterestCreateRequest request)
        {
            return new Interest
            {
                Name = request.Name
            };
        }

        public static InterestResponse ToResponse(this Interest entity)
        {
            return new InterestResponse
            {
                InterestId = entity.InterestId,
                Name = entity.Name
            };
        }

        public static EmployeeInterest ToEntity(this EmployeeInterestAssignRequest request, long employeeId)
        {
            return new EmployeeInterest
            {
                EmployeeId = employeeId,
                InterestId = request.InterestId
            };
        }

        public static EmployeeInterestResponse ToResponse(this EmployeeInterest entity)
        {
            return new EmployeeInterestResponse
            {
                EmployeeInterestId = entity.EmployeeInterestId,
                EmployeeId = entity.EmployeeId,
                InterestId = entity.InterestId
            };
        }
    }
}
