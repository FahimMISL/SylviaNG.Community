namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskAttachmentAddRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long UploadedBy { get; set; }
    }
}
