using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class TaskAttachment : Audit
{
    public long AttachmentId { get; set; }
    public long TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public long UploadedBy { get; set; }
}
