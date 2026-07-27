using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeBadgeRepository : IRepository<EmployeeBadge>
    {
        Task<List<EmployeeBadge>> GetByEmployeeIdAsync(long employeeId);
    }
}
