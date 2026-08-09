using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupPostGetAllPaged
{
    public class GroupPostGetAllPagedHandler : IRequestHandler<GroupPostGetAllPagedQuery, PagedResult<PostResponse>>
    {
        private readonly IGroupService _groupService;

        public GroupPostGetAllPagedHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<PagedResult<PostResponse>> Handle(GroupPostGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetPostsAsync(query.GroupId, query.Request, query.CallerEmployeeId, query.IsHrOrAdmin);
        }
    }
}
