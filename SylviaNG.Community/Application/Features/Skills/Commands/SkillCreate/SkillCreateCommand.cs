using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;

namespace SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate
{
    public class SkillCreateCommand : IRequest<long>
    {
        public SkillCreateRequest Request { get; set; }

        public SkillCreateCommand(SkillCreateRequest request)
        {
            Request = request;
        }
    }
}
