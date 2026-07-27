using MediatR;

namespace SylviaNG.Community.Application.Features.Skills.Commands.SkillDelete
{
    public class SkillDeleteCommand : IRequest
    {
        public long SkillId { get; set; }

        public SkillDeleteCommand(long skillId)
        {
            SkillId = skillId;
        }
    }
}
