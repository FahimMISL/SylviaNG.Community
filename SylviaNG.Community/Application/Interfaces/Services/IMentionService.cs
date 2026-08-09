using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IMentionService
    {
        Task<long> CreateAsync(MentionCreateRequest request);

        /// <summary>
        /// Fan-out entry point for auto-detected @mentions from a post/comment composer:
        /// dedupes self-mentions and duplicate IDs, then creates one Mention (and one
        /// notification, via CreateAsync) per remaining employee.
        /// </summary>
        Task CreateMentionsAsync(string entityType, long entityId, long authorEmployeeId, IEnumerable<long>? mentionedEmployeeIds);

        Task<PagedResult<MentionResponse>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request);

        Task<List<MentionResponse>> GetByEntityAsync(string entityType, long entityId);
    }
}
