using MediatR;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Queries.EmployeeSkillGetAll
{
    public class EmployeeSkillGetAllQuery : IRequest<List<EmployeeSkillResponse>>
    {
        public long EmployeeId { get; set; }

        public EmployeeSkillGetAllQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
