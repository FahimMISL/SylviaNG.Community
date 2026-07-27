using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Shared paging/filter request for both the Directory (US-1.1/1.2, always active-only)
    /// and the HR/Admin Management list (US-1.7, all statuses). IsActive is only honored by
    /// the management endpoint - the directory endpoint always forces active-only server-side.
    /// </summary>
    public class EmployeeFilterRequest : PagedRequest
    {
        public long? DepartmentId { get; set; }
        public long? SiteId { get; set; }
        public long? DesignationId { get; set; }
        public bool? IsActive { get; set; }
    }
}
