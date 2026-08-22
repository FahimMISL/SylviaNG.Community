namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskCreateRequest
    {
        /// <summary>Null creates an individual (non-team) task, assigned directly to an employee (US-7.6).</summary>
        public long? TeamId { get; set; }
        public long AssignedBy { get; set; }
        public long AssignedTo { get; set; }
        public long? RecurringTaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int? ReminderDays { get; set; }
    }
}
