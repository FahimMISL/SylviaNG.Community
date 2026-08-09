using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    /// <summary>
    /// TaskHistory is an insert-only log table - only AddAsync (inherited) and read access
    /// are exposed here; there is no Update/Delete usage for this entity.
    /// </summary>
    public interface ITaskHistoryRepository : IRepository<TaskHistory>
    {
        Task<List<TaskHistory>> GetByTaskIdAsync(long taskId);
    }
}
