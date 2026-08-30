using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class ChatMessageMapper
    {
        public static ChatMessage ToEntity(this ChatMessageSendRequest request, long conversationId, long senderEmployeeId, DateTime sentAt)
        {
            return new ChatMessage
            {
                ChatConversationId = conversationId,
                SenderEmployeeId = senderEmployeeId,
                Body = request.Body,
                MessageType = request.MessageType,
                ReplyToMessageId = request.ReplyToMessageId,
                SentAt = sentAt
            };
        }

        public static ChatMessageResponse ToResponse(
            this ChatMessage entity,
            string senderName,
            List<ChatMessageAttachmentResponse>? attachments = null,
            List<ChatMessageReactionResponse>? reactions = null,
            ChatMessageReplyPreviewResponse? replyTo = null)
        {
            // A removed message's content is never handed back to the client, even though the
            // row itself (and its attachments/reactions in the DB) are kept for audit purposes.
            var isDeleted = entity.IsDeleted;

            return new ChatMessageResponse
            {
                ChatMessageId = entity.ChatMessageId,
                ChatConversationId = entity.ChatConversationId,
                SenderEmployeeId = entity.SenderEmployeeId,
                SenderName = senderName,
                Body = isDeleted ? null : entity.Body,
                MessageType = entity.MessageType,
                SharedContentType = entity.SharedContentType,
                SharedContentId = entity.SharedContentId,
                SentAt = entity.SentAt,
                Attachments = isDeleted ? new List<ChatMessageAttachmentResponse>() : (attachments ?? new List<ChatMessageAttachmentResponse>()),
                Reactions = reactions ?? new List<ChatMessageReactionResponse>(),
                IsDeleted = isDeleted,
                IsForwarded = entity.IsForwarded,
                ReplyTo = isDeleted ? null : replyTo
            };
        }

        public static ChatMessageReplyPreviewResponse ToReplyPreview(this ChatMessage entity, string senderName, bool hasAttachment)
        {
            const int PreviewMaxLength = 120;
            var body = entity.Body;
            var bodyPreview = entity.IsDeleted
                ? null
                : (body != null && body.Length > PreviewMaxLength ? body[..PreviewMaxLength] + "..." : body);

            return new ChatMessageReplyPreviewResponse
            {
                ChatMessageId = entity.ChatMessageId,
                SenderEmployeeId = entity.SenderEmployeeId,
                SenderName = senderName,
                BodyPreview = bodyPreview,
                HasAttachment = !entity.IsDeleted && hasAttachment,
                IsDeleted = entity.IsDeleted
            };
        }

        public static ChatMessageReactionResponse ToResponse(this ChatMessageReaction entity, long chatConversationId)
        {
            return new ChatMessageReactionResponse
            {
                ChatMessageReactionId = entity.ChatMessageReactionId,
                ChatConversationId = chatConversationId,
                ChatMessageId = entity.ChatMessageId,
                EmployeeId = entity.EmployeeId,
                ReactionType = entity.ReactionType
            };
        }

        public static ChatMessageAttachment ToEntity(this ChatMessageAttachmentRequest request, long chatMessageId)
        {
            return new ChatMessageAttachment
            {
                ChatMessageId = chatMessageId,
                FileStorageId = request.FileStorageId,
                AttachmentType = request.AttachmentType,
                DurationSeconds = request.DurationSeconds
            };
        }

        public static ChatMessageAttachmentResponse ToResponse(this ChatMessageAttachment entity, FileStorage file)
        {
            return new ChatMessageAttachmentResponse
            {
                ChatMessageAttachmentId = entity.ChatMessageAttachmentId,
                FileStorageId = entity.FileStorageId,
                OriginalFileName = file.OriginalFileName,
                StoragePath = file.StoragePath,
                MimeType = file.MimeType,
                FileSize = file.FileSize,
                AttachmentType = entity.AttachmentType,
                DurationSeconds = entity.DurationSeconds
            };
        }
    }
}
