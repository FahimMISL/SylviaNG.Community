using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeSkillRepository : IRepository<EmployeeSkill>
    {
        Task<bool> ExistsAsync(long employeeId, long skillId);
        Task<EmployeeSkill?> GetAsync(long employeeId, long skillId);
        Task<List<EmployeeSkill>> GetByEmployeeIdAsync(long employeeId);
    }
}
