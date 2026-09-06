using System.Text.Json.Serialization;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostResponse
    {
        public long PostId { get; set; }
        public long EmployeeId { get; set; }
        public long? GroupId { get; set; }
        /// <summary>Safe to reveal even when CanView is false - naming a group isn't sensitive, only its posts are.</summary>
        public string? GroupName { get; set; }
        /// <summary>False when this is a Private group's post and the caller isn't an active member (or HR/Admin) - Content is nulled out in that case rather than the request being rejected outright, so a "Join {GroupName} to see this post" UI can still be built from a single 200 response.</summary>
        public bool CanView { get; set; } = true;
        public string Type { get; set; } = string.Empty;
        public VisibilityEnum Visibility { get; set; }
        public string? Content { get; set; }
        public bool IsAnnouncement { get; set; }
        public bool IsPoll { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }
        [JsonConverter(typeof(NullableUtcDateTimeJsonConverter))]
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
    }
}
