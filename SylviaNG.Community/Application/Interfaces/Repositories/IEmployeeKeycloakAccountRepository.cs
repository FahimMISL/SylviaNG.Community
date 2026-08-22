using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeKeycloakAccountRepository : IRepository<EmployeeKeycloakAccount>
    {
        Task<bool> ExistsByEmployeeIdAsync(long employeeId);
        Task<EmployeeKeycloakAccount?> GetByEmployeeIdAsync(long employeeId);

        /// <summary>Batched existence check - which of the given employee IDs already have a Keycloak account.</summary>
        Task<HashSet<long>> GetEmployeeIdsWithAccountsAsync(IEnumerable<long> employeeIds);
    }
}
