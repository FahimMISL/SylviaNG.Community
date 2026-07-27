using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITaskAttachmentRepository : IRepository<TaskAttachment>
    {
        Task<List<TaskAttachment>> GetByTaskIdAsync(long taskId);
    }
}
