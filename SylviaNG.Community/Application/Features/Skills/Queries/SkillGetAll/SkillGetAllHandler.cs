using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAll
{
    public class SkillGetAllHandler : IRequestHandler<SkillGetAllQuery, List<SkillResponse>>
    {
        private readonly ISkillService _skillService;

        public SkillGetAllHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<List<SkillResponse>> Handle(SkillGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _skillService.GetAllAsync();
        }
    }
}
