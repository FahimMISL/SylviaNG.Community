using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Feed sidebar "Today's Events" card row. Deliberately never carries a birth year - the
    /// company-wide feed widget only ever shows that today IS someone's birthday, not how old
    /// they are.
    /// </summary>
    public class TodayEventResponse
    {
        public long EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? PhotoUrl { get; set; }
        public TodayEventTypeEnum EventType { get; set; }

        /// <summary>Only set when EventType == Anniversary.</summary>
        public int? YearsOfService { get; set; }
    }
}
