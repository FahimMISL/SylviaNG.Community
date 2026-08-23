using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class PostMapper
    {
        public static Post ToEntity(this PostCreateRequest request)
        {
            return new Post
            {
                EmployeeId = request.EmployeeId,
                GroupId = request.GroupId,
                Type = request.Type,
                Visibility = request.Visibility,
                Content = request.Content,
                IsAnnouncement = request.IsAnnouncement,
                IsPoll = request.IsPoll,
                IsLocked = false,
                IsHidden = false
            };
        }

        public static void ApplyUpdate(this Post entity, PostUpdateRequest request)
        {
            if (request.Type != null) entity.Type = request.Type;
            if (request.Visibility.HasValue) entity.Visibility = request.Visibility.Value;
            if (request.Content != null) entity.Content = request.Content;
            if (request.IsAnnouncement.HasValue) entity.IsAnnouncement = request.IsAnnouncement.Value;
        }

        public static PostResponse ToResponse(this Post entity)
        {
            return new PostResponse
            {
                PostId = entity.PostId,
                EmployeeId = entity.EmployeeId,
                GroupId = entity.GroupId,
                Type = entity.Type,
                Visibility = entity.Visibility,
                Content = entity.Content,
                IsAnnouncement = entity.IsAnnouncement,
                IsPoll = entity.IsPoll,
                IsLocked = entity.IsLocked,
                IsHidden = entity.IsHidden,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy
            };
        }
    }
}
