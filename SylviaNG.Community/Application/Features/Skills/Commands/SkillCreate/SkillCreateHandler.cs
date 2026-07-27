using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate
{
    public class SkillCreateHandler : IRequestHandler<SkillCreateCommand, long>
    {
        private readonly ISkillService _skillService;

        public SkillCreateHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<long> Handle(SkillCreateCommand command, CancellationToken cancellationToken)
        {
            return await _skillService.CreateAsync(command.Request);
        }
    }
}
