using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITaskRepository : IRepository<TaskEntity>
    {
        Task<PagedResult<TaskEntity>> GetPaginatedAsync(TaskFilterRequest request);
    }
}
