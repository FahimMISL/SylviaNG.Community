using MediatR;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillRemove
{
    public class EmployeeSkillRemoveCommand : IRequest
    {
        public long EmployeeId { get; set; }
        public long SkillId { get; set; }

        public EmployeeSkillRemoveCommand(long employeeId, long skillId)
        {
            EmployeeId = employeeId;
            SkillId = skillId;
        }
    }
}
