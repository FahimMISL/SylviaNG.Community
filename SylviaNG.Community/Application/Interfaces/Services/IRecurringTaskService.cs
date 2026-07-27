using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IRecurringTaskService
    {
        Task<long> CreateAsync(RecurringTaskCreateRequest request);
        Task UpdateAsync(long recurringTaskId, RecurringTaskUpdateRequest request);
        Task DeleteAsync(long recurringTaskId);
        Task<RecurringTaskResponse> GetByIdAsync(long recurringTaskId);
        Task<PagedResult<RecurringTaskResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
