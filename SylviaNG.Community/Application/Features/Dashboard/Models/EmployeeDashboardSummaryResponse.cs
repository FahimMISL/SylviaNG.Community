using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Features.Dashboard.Models
{
    /// <summary>US-8.1: the widgets every employee sees on their personal dashboard.</summary>
    public class EmployeeDashboardSummaryResponse
    {
        public int TeamCount { get; set; }
        public int PendingSurveyCount { get; set; }
        public int RecognitionsReceivedCount { get; set; }
        public int OpenTaskCount { get; set; }
        public List<NotificationResponse> RecentNotifications { get; set; } = new();

        /// <summary>US-8.2 gate: whether the caller supervises at least one team - drives whether the
        /// frontend also calls GET community/dashboard/supervisor-task-overview.</summary>
        public bool IsSupervisor { get; set; }
    }
}
