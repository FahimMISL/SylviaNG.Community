using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetById
{
    public class SkillGetByIdQuery : IRequest<SkillResponse>
    {
        public long SkillId { get; set; }

        public SkillGetByIdQuery(long skillId)
        {
            SkillId = skillId;
        }
    }
}
