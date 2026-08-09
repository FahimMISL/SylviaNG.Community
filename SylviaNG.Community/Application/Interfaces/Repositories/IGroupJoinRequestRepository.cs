using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IGroupJoinRequestRepository : IRepository<GroupJoinRequest>
    {
        Task<GroupJoinRequest?> GetPendingAsync(long groupId, long employeeId);
        Task<List<GroupJoinRequest>> GetPendingByGroupIdAsync(long groupId);
    }
}
