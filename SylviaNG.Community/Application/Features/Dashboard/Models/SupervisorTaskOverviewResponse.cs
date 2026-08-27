namespace SylviaNG.Community.Application.Features.Dashboard.Models
{
    /// <summary>US-8.2: task status breakdown across everything the caller has assigned (team-scoped and individual).</summary>
    public class SupervisorTaskOverviewResponse
    {
        public int Total { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Overdue { get; set; }
    }
}
