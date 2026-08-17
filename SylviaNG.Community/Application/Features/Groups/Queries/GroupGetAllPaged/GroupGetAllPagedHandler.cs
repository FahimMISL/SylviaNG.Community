using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetAllPaged
{
    public class GroupGetAllPagedHandler : IRequestHandler<GroupGetAllPagedQuery, PagedResult<GroupResponse>>
    {
        private readonly IGroupService _groupService;

        public GroupGetAllPagedHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<PagedResult<GroupResponse>> Handle(GroupGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetPaginatedAsync(query.Request);
        }
    }
}
