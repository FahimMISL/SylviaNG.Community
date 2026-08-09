using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IPollService
    {
        Task<long> CreateAsync(long postId, PollCreateRequest request);
        Task<PollResponse> GetByPostIdAsync(long postId);
        Task<long> VoteAsync(long postId, PollVoteRequest request);
    }
}
