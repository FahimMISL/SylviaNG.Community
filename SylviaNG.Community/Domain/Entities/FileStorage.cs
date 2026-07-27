using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Metadata-only record of an uploaded file. Byte storage/upload handling is out of scope -
/// this just tracks where a file lives (StoragePath) and what it is. EntityId is a generic,
/// non-FK polymorphic pointer to whatever record the file is attached to.
/// </summary>
public class FileStorage : Audit
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
}
