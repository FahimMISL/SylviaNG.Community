namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    public class ChatMessageForwardRequest
    {
        /// <summary>Conversations the caller is a participant of; targets they aren't a member of are silently skipped.</summary>
        public List<long> ConversationIds { get; set; } = new();
    }
}
