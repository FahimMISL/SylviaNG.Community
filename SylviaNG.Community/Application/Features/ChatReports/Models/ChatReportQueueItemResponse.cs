namespace SylviaNG.Community.Application.Features.ChatReports.Models
{
    /// <summary>
    /// Enriched moderation-queue row - a plain ChatReport plus enough context (conversation title,
    /// reported message preview, reporter/sender names) for HR/Admin to act on a report without a
    /// separate drill-through call per row, mirroring ContentReportQueueItemResponse.
    /// </summary>
    public class ChatReportQueueItemResponse
    {
        public long ReportId { get; set; }
        public long ReportedByEmployeeId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public long ChatConversationId { get; set; }
        public string ConversationTitle { get; set; } = string.Empty;
        public string ConversationType { get; set; } = string.Empty;
        public long? ChatMessageId { get; set; }
        public string MessageBodyPreview { get; set; } = string.Empty;
        public bool IsMessageDeleted { get; set; }
        public long SenderEmployeeId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
