using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// References FileStorage by FK (FileStorageId is the enforced source of truth), with FilePath
/// denormalized alongside it so this table stays queryable without a join, matching
/// PostAttachment/TaskAttachment/ListingImage. Voice notes are just an audio blob with
/// AttachmentType = Voice, uploaded through the existing FileUploadController.
/// </summary>
public class ChatMessageAttachment : Audit
{
    public long ChatMessageAttachmentId { get; set; }
    public long ChatMessageId { get; set; }
    public long FileStorageId { get; set; }
    public ChatAttachmentTypeEnum AttachmentType { get; set; }
    public int? DurationSeconds { get; set; }

    /// <summary>Denormalized copy of FileStorage.StoragePath at attachment-creation time, for
    /// parity with PostAttachment/TaskAttachment/ListingImage (all queryable without a join).
    /// FileStorageId remains the source of truth and the FK actually enforced by the DB.</summary>
    public string FilePath { get; set; } = string.Empty;
}
