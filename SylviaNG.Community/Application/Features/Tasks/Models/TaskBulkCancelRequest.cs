namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskBulkCancelRequest
    {
        public List<long> TaskIds { get; set; } = new();
    }
}
