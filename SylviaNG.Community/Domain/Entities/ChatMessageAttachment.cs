using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// References FileStorage by FK rather than duplicating filename/path/size inline (the
/// older PostAttachment/TaskAttachment style) - Messenger is a brand-new slice built
/// directly on the current-generation upload pattern. Wired up in the attachments branch;
/// the table ships now alongside the rest of the module's schema. Voice notes are just an
/// audio blob with AttachmentType = Voice, uploaded through the existing FileUploadController.
/// </summary>
public class ChatMessageAttachment : Audit
{
    public long ChatMessageAttachmentId { get; set; }
    public long ChatMessageId { get; set; }
    public long FileStorageId { get; set; }
    public ChatAttachmentTypeEnum AttachmentType { get; set; }
    public int? DurationSeconds { get; set; }
}
