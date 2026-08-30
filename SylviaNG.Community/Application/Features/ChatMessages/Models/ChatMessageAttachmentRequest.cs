using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    /// <summary>
    /// References a file already uploaded via the existing community/file-upload endpoint
    /// (module "Messenger") - the client uploads first, then sends the returned FileId here.
    /// </summary>
    public class ChatMessageAttachmentRequest
    {
        public long FileStorageId { get; set; }
        public ChatAttachmentTypeEnum AttachmentType { get; set; }
        /// <summary>Voice attachments only.</summary>
        public int? DurationSeconds { get; set; }
    }
}
