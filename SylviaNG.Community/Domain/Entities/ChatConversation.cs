using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A Messenger conversation - one table for both 1:1 (Direct) and group chat, discriminated
/// by Type, since participants/messages/moderation are identical between them; only Title/
/// GroupAvatarFileId are meaningful for Group. LastMessageAt/LastMessagePreview are
/// denormalized on send so the inbox list never has to join ChatMessages.
/// </summary>
public class ChatConversation : Audit
{
    public long ChatConversationId { get; set; }
    public ConversationTypeEnum Type { get; set; } = ConversationTypeEnum.Direct;
    public string? Title { get; set; }
    public long? GroupAvatarFileId { get; set; }
    public long CreatedByEmployeeId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessagePreview { get; set; }
}
