using MediatR;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillAssign
{
    public class EmployeeSkillAssignCommand : IRequest<long>
    {
        public long EmployeeId { get; set; }
        public EmployeeSkillAssignRequest Request { get; set; }

        public EmployeeSkillAssignCommand(long employeeId, EmployeeSkillAssignRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
