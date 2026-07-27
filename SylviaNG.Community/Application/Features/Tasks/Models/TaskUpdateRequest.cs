namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskUpdateRequest
    {
        public long? TeamId { get; set; }
        public long? AssignedTo { get; set; }
        public long? RecurringTaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int? ReminderDays { get; set; }
    }
}
