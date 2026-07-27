using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITaskCommentRepository : IRepository<TaskComment>
    {
        Task<List<TaskComment>> GetByTaskIdAsync(long taskId);
    }
}
