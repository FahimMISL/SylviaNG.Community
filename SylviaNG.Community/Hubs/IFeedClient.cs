using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Strongly-typed client contract for <see cref="FeedHub"/>.
    /// Defines the methods the server can invoke on connected clients.
    /// </summary>
    public interface IFeedClient
    {
        // The trailing CancellationToken is SignalR's typed-client idiom: it is used only to
        // cancel the local send operation and is stripped before the call is sent over the wire.
        Task ReceivePollResults(PollResponse pollResponse, CancellationToken cancellationToken = default);
    }
}
