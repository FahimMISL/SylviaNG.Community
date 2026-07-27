namespace SylviaNG.Community.Application.Features.RecurringTasks.Models
{
    public class RecurringTaskUpdateRequest
    {
        public string? Frequency { get; set; }
        public int? IntervalValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
