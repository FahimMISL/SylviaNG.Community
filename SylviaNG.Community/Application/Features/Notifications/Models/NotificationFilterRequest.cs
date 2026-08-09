using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Notifications.Models
{
    /// <summary>
    /// Paging/filter request for the Notification Center - read-state and category
    /// filters on top of the inherited pagination fields (the "Unread" tab sends
    /// IsRead=false, category dropdown sends Category).
    /// </summary>
    public class NotificationFilterRequest : PagedRequest
    {
        public bool? IsRead { get; set; }
        public string? Category { get; set; }
    }
}
