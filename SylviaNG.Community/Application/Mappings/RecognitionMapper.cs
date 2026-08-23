using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class RecognitionMapper
    {
        public static Recognition ToEntity(this RecognitionCreateRequest request, long senderId)
        {
            return new Recognition
            {
                SenderId = senderId,
                RecipientId = request.RecipientId,
                RecognitionType = request.RecognitionType,
                CoreValue = request.CoreValue,
                AwardTitle = request.AwardTitle,
                Message = request.Message,
                IsPublic = request.IsPublic,
                IsHrIssued = request.IsHrIssued
            };
        }

        public static RecognitionResponse ToResponse(this Recognition entity, IEnumerable<Badge>? badges = null)
        {
            return new RecognitionResponse
            {
                RecognitionId = entity.RecognitionId,
                SenderId = entity.SenderId,
                RecipientId = entity.RecipientId,
                Badges = badges?.Select(b => b.ToResponse()).ToList() ?? new(),
                RecognitionType = entity.RecognitionType,
                CoreValue = entity.CoreValue,
                AwardTitle = entity.AwardTitle,
                Message = entity.Message,
                IsPublic = entity.IsPublic,
                IsHrIssued = entity.IsHrIssued,
                CreatedAt = entity.CreatedAt
            };
        }

        public static RecognitionReaction ToEntity(this RecognitionReactionAddRequest request, long recognitionId, long employeeId)
        {
            return new RecognitionReaction
            {
                RecognitionId = recognitionId,
                EmployeeId = employeeId,
                ReactionType = request.ReactionType
            };
        }

        public static RecognitionReactionResponse ToResponse(this RecognitionReaction entity)
        {
            return new RecognitionReactionResponse
            {
                ReactionId = entity.ReactionId,
                RecognitionId = entity.RecognitionId,
                EmployeeId = entity.EmployeeId,
                ReactionType = entity.ReactionType
            };
        }

        public static RecognitionComment ToEntity(this RecognitionCommentAddRequest request, long recognitionId, long employeeId)
        {
            return new RecognitionComment
            {
                RecognitionId = recognitionId,
                EmployeeId = employeeId,
                ParentCommentId = request.ParentCommentId,
                Comment = request.Comment
            };
        }

        public static RecognitionCommentResponse ToResponse(this RecognitionComment entity)
        {
            return new RecognitionCommentResponse
            {
                CommentId = entity.CommentId,
                RecognitionId = entity.RecognitionId,
                EmployeeId = entity.EmployeeId,
                ParentCommentId = entity.ParentCommentId,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
