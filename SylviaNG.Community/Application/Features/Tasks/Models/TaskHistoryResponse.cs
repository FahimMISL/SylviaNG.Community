namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskHistoryResponse
    {
        public long HistoryId { get; set; }
        public long TaskId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long ChangedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
