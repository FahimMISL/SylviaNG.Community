using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetById
{
    public class SkillGetByIdHandler : IRequestHandler<SkillGetByIdQuery, SkillResponse>
    {
        private readonly ISkillService _skillService;

        public SkillGetByIdHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<SkillResponse> Handle(SkillGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _skillService.GetByIdAsync(query.SkillId);
        }
    }
}
