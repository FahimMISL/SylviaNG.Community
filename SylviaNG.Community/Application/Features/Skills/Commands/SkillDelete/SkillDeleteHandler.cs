using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Skills.Commands.SkillDelete
{
    public class SkillDeleteHandler : IRequestHandler<SkillDeleteCommand>
    {
        private readonly ISkillService _skillService;

        public SkillDeleteHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task Handle(SkillDeleteCommand command, CancellationToken cancellationToken)
        {
            await _skillService.DeleteAsync(command.SkillId);
        }
    }
}
