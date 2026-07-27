namespace SylviaNG.Community.Application.Features.ContentReports.Models
{
    public class ContentReportResponse
    {
        public long ReportId { get; set; }
        public long ReportedBy { get; set; }
        public long PostId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
