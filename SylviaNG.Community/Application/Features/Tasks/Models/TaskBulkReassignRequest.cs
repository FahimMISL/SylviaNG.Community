namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskBulkReassignRequest
    {
        public List<long> TaskIds { get; set; } = new();
        public long NewAssignedTo { get; set; }
    }
}
