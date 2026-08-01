using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    /// <summary>
    /// Pushes real-time feed events (e.g. live poll results) to clients currently viewing a post,
    /// over SignalR. Implemented in Infrastructure (SignalRFeedBroadcaster) since the concrete
    /// implementation depends on Microsoft.AspNetCore.SignalR.
    /// </summary>
    public interface IFeedBroadcaster
    {
        Task BroadcastPollResultsAsync(long postId, PollResponse pollResponse, CancellationToken cancellationToken = default);
    }
}
