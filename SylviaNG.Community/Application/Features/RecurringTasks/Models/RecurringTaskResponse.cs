namespace SylviaNG.Community.Application.Features.RecurringTasks.Models
{
    public class RecurringTaskResponse
    {
        public long RecurringTaskId { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public int IntervalValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
