using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupJoinRequestGetAllPending
{
    public class GroupJoinRequestGetAllPendingHandler : IRequestHandler<GroupJoinRequestGetAllPendingQuery, List<GroupJoinRequestResponse>>
    {
        private readonly IGroupService _groupService;

        public GroupJoinRequestGetAllPendingHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<List<GroupJoinRequestResponse>> Handle(GroupJoinRequestGetAllPendingQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetPendingJoinRequestsAsync(query.GroupId, query.CallerEmployeeId, query.IsHrOrAdmin);
        }
    }
}
