using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Group-based hub for live feed updates (e.g. poll results). Unlike <see cref="NotificationHub"/>,
    /// which targets a specific employee for the lifetime of their login session, group membership here
    /// is scoped to whichever post(s) a client currently has open, so clients explicitly join/leave a
    /// "post-{postId}" group as they navigate the feed.
    /// </summary>
    [Authorize]
    public class FeedHub : Hub<IFeedClient>
    {
        public async Task JoinPostGroup(long postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");
        }

        public async Task LeavePostGroup(long postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"post-{postId}");
        }
    }
}
