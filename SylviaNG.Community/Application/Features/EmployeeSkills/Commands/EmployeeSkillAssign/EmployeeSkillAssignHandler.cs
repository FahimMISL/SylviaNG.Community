using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillAssign
{
    public class EmployeeSkillAssignHandler : IRequestHandler<EmployeeSkillAssignCommand, long>
    {
        private readonly ISkillService _skillService;

        public EmployeeSkillAssignHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<long> Handle(EmployeeSkillAssignCommand command, CancellationToken cancellationToken)
        {
            return await _skillService.AssignToEmployeeAsync(command.EmployeeId, command.Request);
        }
    }
}
