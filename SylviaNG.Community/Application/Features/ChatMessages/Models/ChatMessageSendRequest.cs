using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    public class ChatMessageSendRequest
    {
        public string? Body { get; set; }
        public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Text;
        public List<ChatMessageAttachmentRequest> Attachments { get; set; } = new();
        public long? ReplyToMessageId { get; set; }
    }
}
