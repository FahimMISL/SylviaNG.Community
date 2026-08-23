namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskResponse
    {
        public long TaskId { get; set; }
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
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        /// <summary>US-7.9: "Completed" | "Overdue" | "DueSoon" | "OnTrack" - computed from DueDate/ReminderDays/Status
        /// at read time in TaskMapper.ToResponse, not persisted.</summary>
        public string DerivedStatus { get; set; } = string.Empty;
    }
}
