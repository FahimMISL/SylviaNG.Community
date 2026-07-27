using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ISkillService
    {
        Task<long> CreateAsync(SkillCreateRequest request);
        Task DeleteAsync(long skillId);
        Task<SkillResponse> GetByIdAsync(long skillId);
        Task<List<SkillResponse>> GetAllAsync();
        Task<PagedResult<SkillResponse>> GetPaginatedAsync(PagedRequest request);
        Task<long> AssignToEmployeeAsync(long employeeId, EmployeeSkillAssignRequest request);
        Task RemoveFromEmployeeAsync(long employeeId, long skillId);
        Task<List<EmployeeSkillResponse>> GetEmployeeSkillsAsync(long employeeId);
    }
}
