namespace SylviaNG.Community.Application.Features.PostAttachments.Models
{
    public class PostAttachmentAddRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
