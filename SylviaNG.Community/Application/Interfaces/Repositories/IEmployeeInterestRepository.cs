using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IEmployeeInterestRepository : IRepository<EmployeeInterest>
    {
        Task<bool> ExistsAsync(long employeeId, long interestId);
        Task<EmployeeInterest?> GetAsync(long employeeId, long interestId);
        Task<List<EmployeeInterest>> GetByEmployeeIdAsync(long employeeId);
    }
}
