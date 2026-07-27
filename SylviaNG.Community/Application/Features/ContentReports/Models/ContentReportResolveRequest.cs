namespace SylviaNG.Community.Application.Features.ContentReports.Models
{
    public class ContentReportResolveRequest
    {
        public long ReviewedBy { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
