using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Mappings
{
    public static class ChatConversationMapper
    {
        public static ChatConversation ToEntity(this ChatConversationCreateRequest request, long callerEmployeeId)
        {
            return new ChatConversation
            {
                Type = request.Type,
                Title = request.Type == ConversationTypeEnum.Group ? request.Title : null,
                CreatedByEmployeeId = callerEmployeeId
            };
        }

        public static ChatConversationResponse ToResponse(this ChatConversation entity, List<ChatParticipantResponse> participants)
        {
            return new ChatConversationResponse
            {
                ChatConversationId = entity.ChatConversationId,
                Type = entity.Type,
                Title = entity.Title,
                GroupAvatarFileId = entity.GroupAvatarFileId,
                CreatedByEmployeeId = entity.CreatedByEmployeeId,
                LastMessageAt = entity.LastMessageAt,
                LastMessagePreview = entity.LastMessagePreview,
                Participants = participants
            };
        }
    }
}
