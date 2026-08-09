using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);

        /// <summary>
        /// Shared by the Directory (activeOnly: true) and HR/Admin Management (activeOnly: false) endpoints.
        /// </summary>
        Task<PagedResult<Employee>> GetPaginatedAsync(EmployeeFilterRequest request, bool activeOnly);

        /// <summary>
        /// Total active employee count, used as the participation-rate denominator for
        /// EntireCompany-scoped survey results.
        /// </summary>
        Task<int> CountActiveAsync();
    }
}
