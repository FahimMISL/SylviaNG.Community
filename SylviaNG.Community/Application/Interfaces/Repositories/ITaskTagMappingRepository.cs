using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITaskTagMappingRepository : IRepository<TaskTagMapping>
    {
        Task<bool> ExistsAsync(long taskId, long tagId);
        Task<TaskTagMapping?> GetAsync(long taskId, long tagId);
        Task<List<TaskTagMapping>> GetByTaskIdAsync(long taskId);
    }
}
