using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAllPaged
{
    public class SkillGetAllPagedQuery : IRequest<PagedResult<SkillResponse>>
    {
        public PagedRequest Request { get; set; }

        public SkillGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
