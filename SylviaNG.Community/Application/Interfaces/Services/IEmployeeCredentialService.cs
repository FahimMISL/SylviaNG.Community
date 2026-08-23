using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IEmployeeCredentialService
    {
        Task<EmployeeCredentialResponse> CreateAsync(EmployeeCredentialCreateRequest request);

        /// <summary>Sets a new temporary password for an employee who already has a Keycloak account
        /// (throws NotFoundException if access was never granted).</summary>
        Task ResetPasswordAsync(long employeeId, string newTemporaryPassword);
    }
}
