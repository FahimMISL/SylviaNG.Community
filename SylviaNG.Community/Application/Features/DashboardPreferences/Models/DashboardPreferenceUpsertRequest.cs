namespace SylviaNG.Community.Application.Features.DashboardPreferences.Models
{
    public class DashboardPreferenceUpsertRequest
    {
        public long EmployeeId { get; set; }
        public string WidgetName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
