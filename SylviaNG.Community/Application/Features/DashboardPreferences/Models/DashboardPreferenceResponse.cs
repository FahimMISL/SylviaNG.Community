namespace SylviaNG.Community.Application.Features.DashboardPreferences.Models
{
    public class DashboardPreferenceResponse
    {
        public long PreferenceId { get; set; }
        public long EmployeeId { get; set; }
        public string WidgetName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public DateTime LastModified { get; set; }
    }
}
