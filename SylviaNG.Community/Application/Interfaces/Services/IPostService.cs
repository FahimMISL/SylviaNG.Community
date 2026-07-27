using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<long> CreateAsync(PostCreateRequest request);
        Task UpdateAsync(long postId, PostUpdateRequest request);
        Task DeleteAsync(long postId);
        Task<PostResponse> GetByIdAsync(long postId);
        Task<PagedResult<PostResponse>> GetFeedPaginatedAsync(PagedRequest request);
        Task SetLockedAsync(long postId, bool isLocked);
        Task SetHiddenAsync(long postId, bool isHidden);
    }
}
