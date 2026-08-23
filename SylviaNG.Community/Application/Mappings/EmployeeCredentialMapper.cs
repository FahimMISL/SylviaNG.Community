using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class EmployeeCredentialMapper
    {
        public static EmployeeCredentialResponse ToResponse(this EmployeeKeycloakAccount entity)
        {
            return new EmployeeCredentialResponse
            {
                EmployeeId = entity.EmployeeId,
                Username = entity.Username,
                KeycloakUserId = entity.KeycloakUserId,
                AssignedRole = entity.AssignedRole
            };
        }
    }
}
