using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class RecognitionMapper
    {
        public static Recognition ToEntity(this RecognitionCreateRequest request)
        {
            return new Recognition
            {
                SenderId = request.SenderId,
                RecipientId = request.RecipientId,
                RecognitionType = request.RecognitionType,
                CoreValue = request.CoreValue,
                AwardTitle = request.AwardTitle,
                Message = request.Message,
                IsPublic = request.IsPublic
            };
        }

        public static RecognitionResponse ToResponse(this Recognition entity)
        {
            return new RecognitionResponse
            {
                RecognitionId = entity.RecognitionId,
                SenderId = entity.SenderId,
                RecipientId = entity.RecipientId,
                RecognitionType = entity.RecognitionType,
                CoreValue = entity.CoreValue,
                AwardTitle = entity.AwardTitle,
                Message = entity.Message,
                IsPublic = entity.IsPublic,
                CreatedAt = entity.CreatedAt
            };
        }

        public static RecognitionReaction ToEntity(this RecognitionReactionAddRequest request, long recognitionId)
        {
            return new RecognitionReaction
            {
                RecognitionId = recognitionId,
                EmployeeId = request.EmployeeId,
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

        public static RecognitionComment ToEntity(this RecognitionCommentAddRequest request, long recognitionId)
        {
            return new RecognitionComment
            {
                RecognitionId = recognitionId,
                EmployeeId = request.EmployeeId,
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
