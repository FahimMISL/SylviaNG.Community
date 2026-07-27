using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAllPaged
{
    public class SkillGetAllPagedHandler : IRequestHandler<SkillGetAllPagedQuery, PagedResult<SkillResponse>>
    {
        private readonly ISkillService _skillService;

        public SkillGetAllPagedHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<PagedResult<SkillResponse>> Handle(SkillGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _skillService.GetPaginatedAsync(query.Request);
        }
    }
}
