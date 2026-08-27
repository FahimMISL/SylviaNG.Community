using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeKeycloakAccountRepository : IRepository<EmployeeKeycloakAccount>
    {
        Task<bool> ExistsByEmployeeIdAsync(long employeeId);
        Task<EmployeeKeycloakAccount?> GetByEmployeeIdAsync(long employeeId);

        /// <summary>
        /// Looks up the local employee link for a Keycloak user by their Keycloak subject ("sub")
        /// - used to resolve EmployeeId for Keycloak-issued tokens that are missing an "employee_id"
        /// claim (see EmployeeIdentityEnrichmentMiddleware). Unlike the "employee_id" claim, the
        /// subject is always present on any valid Keycloak token, regardless of realm attribute config.
        /// </summary>
        Task<EmployeeKeycloakAccount?> GetByKeycloakUserIdAsync(string keycloakUserId);

        /// <summary>Batched existence check - which of the given employee IDs already have a Keycloak account.</summary>
        Task<HashSet<long>> GetEmployeeIdsWithAccountsAsync(IEnumerable<long> employeeIds);

        /// <summary>Active accounts whose AssignedRole is one of the given roles (e.g. "HR"/"Admin") -
        /// the only queryable source of "who currently has this access level" in this codebase.</summary>
        Task<List<long>> GetEmployeeIdsByRolesAsync(IEnumerable<string> roles);
    }
}
