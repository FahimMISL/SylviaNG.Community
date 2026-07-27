namespace SylviaNG.Community.Application.Features.PostAttachments.Models
{
    public class PostAttachmentResponse
    {
        public long AttachmentId { get; set; }
        public long PostId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
