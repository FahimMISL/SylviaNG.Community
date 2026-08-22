namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskReportResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "text/plain";
    }
}
