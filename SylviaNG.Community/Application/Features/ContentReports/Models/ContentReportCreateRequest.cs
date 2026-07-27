namespace SylviaNG.Community.Application.Features.ContentReports.Models
{
    public class ContentReportCreateRequest
    {
        public long ReportedBy { get; set; }
        public long PostId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
