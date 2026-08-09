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
