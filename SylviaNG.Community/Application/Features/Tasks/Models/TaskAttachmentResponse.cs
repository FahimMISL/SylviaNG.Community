namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskAttachmentResponse
    {
        public long AttachmentId { get; set; }
        public long TaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long UploadedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
