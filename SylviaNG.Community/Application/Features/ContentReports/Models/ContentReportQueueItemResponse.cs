namespace SylviaNG.Community.Application.Features.ContentReports.Models
{
    /// <summary>
    /// Enriched moderation-queue row - a plain ContentReportResponse plus enough context
    /// (content preview, reporter/author names, current post state) for HR/Admin to act on
    /// a report without a separate drill-through call per row (US-3.11).
    /// </summary>
    public class ContentReportQueueItemResponse
    {
        public long ReportId { get; set; }
        public long ReportedBy { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public long PostId { get; set; }
        public string? PostContentPreview { get; set; }
        public string PostType { get; set; } = string.Empty;
        public long PostAuthorId { get; set; }
        public string PostAuthorName { get; set; } = string.Empty;
        public bool IsPostHidden { get; set; }
        public bool IsPostLocked { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
