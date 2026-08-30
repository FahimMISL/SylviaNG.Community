namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    /// <summary>Admin-only partial update - null fields are left unchanged.</summary>
    public class ChatConversationUpdateGroupRequest
    {
        public string? Title { get; set; }

        /// <summary>Id returned by a prior community/file-upload call (module "messenger-group-avatar").</summary>
        public long? GroupAvatarFileId { get; set; }
    }
}
