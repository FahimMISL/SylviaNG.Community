using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ITaskTagService
    {
        Task<long> CreateAsync(TaskTagCreateRequest request);
        Task DeleteAsync(long tagId);
        Task<TaskTagResponse> GetByIdAsync(long tagId);
        Task<PagedResult<TaskTagResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
