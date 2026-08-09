using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeContactLinkRepository : IRepository<EmployeeContactLink>
    {
        Task<List<EmployeeContactLink>> GetByEmployeeIdAsync(long employeeId);
    }
}
