using Microsoft.AspNetCore.SignalR;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Hubs;

namespace SylviaNG.Community.Infrastructure.Services
{
    /// <summary>
    /// Lives in Infrastructure because it depends on Microsoft.AspNetCore.SignalR
    /// (same placement logic as SignalRNotificationBroadcaster).
    /// </summary>
    public class SignalRFeedBroadcaster : IFeedBroadcaster
    {
        private readonly IHubContext<FeedHub, IFeedClient> _hubContext;

        public SignalRFeedBroadcaster(IHubContext<FeedHub, IFeedClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastPollResultsAsync(long postId, PollResponse pollResponse, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"post-{postId}").ReceivePollResults(pollResponse, cancellationToken);
        }
    }
}
