using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for the Announcement entity.
    /// Follow this pattern for all new feature mappings.
    /// </summary>
    public static class AnnouncementMapper
    {
        public static Announcement ToEntity(this AnnouncementCreateRequest request)
        {
            return new Announcement
            {
                SiteId = request.SiteId,
                DepartmentId = request.DepartmentId,
                Title = request.Title,
                Body = request.Body,
                AnnouncementType = request.AnnouncementType,
                PublishDate = request.PublishDate,
                ExpiryDate = request.ExpiryDate,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Announcement entity, AnnouncementUpdateRequest request)
        {
            if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId;
            if (request.Title != null) entity.Title = request.Title;
            if (request.Body != null) entity.Body = request.Body;
            if (request.AnnouncementType.HasValue) entity.AnnouncementType = request.AnnouncementType.Value;
            if (request.Status.HasValue) entity.Status = request.Status.Value;
            if (request.PublishDate.HasValue) entity.PublishDate = request.PublishDate;
            if (request.ExpiryDate.HasValue) entity.ExpiryDate = request.ExpiryDate;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static AnnouncementResponse ToResponse(this Announcement entity)
        {
            return new AnnouncementResponse
            {
                AnnouncementId = entity.AnnouncementId,
                SiteId = entity.SiteId,
                DepartmentId = entity.DepartmentId,
                Title = entity.Title,
                Body = entity.Body,
                AnnouncementType = entity.AnnouncementType,
                Status = entity.Status,
                PublishDate = entity.PublishDate,
                ExpiryDate = entity.ExpiryDate,
                IsActive = entity.IsActive
            };
        }

        public static AnnouncementLookupResponse ToLookupResponse(this Announcement entity)
        {
            return new AnnouncementLookupResponse
            {
                AnnouncementId = entity.AnnouncementId,
                Title = entity.Title
            };
        }
    }
}