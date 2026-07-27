namespace SylviaNG.Community.Application.Features.FileStorages.Models
{
    public class FileStorageResponse
    {
        public long FileId { get; set; }
        public string Module { get; set; } = string.Empty;
        public long? EntityId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public long UploadedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
