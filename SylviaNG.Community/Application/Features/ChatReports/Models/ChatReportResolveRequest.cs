namespace SylviaNG.Community.Application.Features.ChatReports.Models
{
    public class ChatReportResolveRequest
    {
        public long ReviewedBy { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
