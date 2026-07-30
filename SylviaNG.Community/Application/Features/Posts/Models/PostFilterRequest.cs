using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Posts.Models
{
    /// <summary>
    /// Paging/filter request for browsing the feed - post-type filter (Announcements/Polls)
    /// plus the inherited free-text search over Content. Visibility scoping is not a
    /// client-chosen filter - it's derived server-side from the caller's identity.
    /// </summary>
    public class PostFilterRequest : PagedRequest
    {
        public bool? IsAnnouncement { get; set; }
        public bool? IsPoll { get; set; }
    }
}
