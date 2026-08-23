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

        /// <summary>
        /// Active employee count for Department/Branch-scoped survey audiences, used as the
        /// participation-rate denominator when the audience isn't EntireCompany. departmentIds and
        /// siteIds (Branch maps to Employee.SiteId - there's no separate Employee.BranchId) are
        /// unioned and de-duplicated, so an employee matching both a targeted department and a
        /// targeted branch is only counted once. Pass an empty collection for whichever dimension
        /// isn't targeted.
        /// </summary>
        Task<int> CountActiveByDepartmentOrSiteIdsAsync(IEnumerable<long> departmentIds, IEnumerable<long> siteIds);
    }
}
