using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    /// <summary>
    /// Paging/filter request for the Task list endpoint - filterable by status, assignee and team.
    /// </summary>
    public class TaskFilterRequest : PagedRequest
    {
        public string? Status { get; set; }
        public long? AssignedTo { get; set; }
        public long? AssignedBy { get; set; }
        public long? TeamId { get; set; }

        /// <summary>When true, restricts to individual (non-team) tasks - i.e. TeamId is null (US-7.7).</summary>
        public bool IndividualOnly { get; set; }
    }
}
