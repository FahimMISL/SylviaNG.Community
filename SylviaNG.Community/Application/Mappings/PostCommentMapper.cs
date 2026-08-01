using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class PostCommentMapper
    {
        public static PostComment ToEntity(this PostCommentAddRequest request, long postId)
        {
            return new PostComment
            {
                PostId = postId,
                EmployeeId = request.EmployeeId,
                ParentCommentId = request.ParentCommentId,
                Content = request.Content
            };
        }

        public static void ApplyUpdate(this PostComment entity, PostCommentUpdateRequest request)
        {
            entity.Content = request.Content;
        }

        public static PostCommentResponse ToResponse(this PostComment entity)
        {
            return new PostCommentResponse
            {
                CommentId = entity.CommentId,
                PostId = entity.PostId,
                EmployeeId = entity.EmployeeId,
                ParentCommentId = entity.ParentCommentId,
                Content = entity.Content,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
