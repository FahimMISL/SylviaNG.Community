using SylviaNG.Community.Application.Features.PostAttachments.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class PostAttachmentMapper
    {
        public static PostAttachment ToEntity(this PostAttachmentAddRequest request, long postId)
        {
            return new PostAttachment
            {
                PostId = postId,
                FileName = request.FileName,
                FileType = request.FileType,
                FilePath = request.FilePath,
                FileSize = request.FileSize,
                UploadedAt = DateTime.UtcNow
            };
        }

        public static PostAttachmentResponse ToResponse(this PostAttachment entity)
        {
            return new PostAttachmentResponse
            {
                AttachmentId = entity.AttachmentId,
                PostId = entity.PostId,
                FileName = entity.FileName,
                FileType = entity.FileType,
                FilePath = entity.FilePath,
                FileSize = entity.FileSize,
                UploadedAt = entity.UploadedAt
            };
        }
    }
}
