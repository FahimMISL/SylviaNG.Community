using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillRemove
{
    public class EmployeeSkillRemoveHandler : IRequestHandler<EmployeeSkillRemoveCommand>
    {
        private readonly ISkillService _skillService;

        public EmployeeSkillRemoveHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task Handle(EmployeeSkillRemoveCommand command, CancellationToken cancellationToken)
        {
            await _skillService.RemoveFromEmployeeAsync(command.EmployeeId, command.SkillId);
        }
    }
}
