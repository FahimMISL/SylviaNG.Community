using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupMemberGetAll
{
    public class GroupMemberGetAllHandler : IRequestHandler<GroupMemberGetAllQuery, List<GroupMemberResponse>>
    {
        private readonly IGroupService _groupService;

        public GroupMemberGetAllHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<List<GroupMemberResponse>> Handle(GroupMemberGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetMembersAsync(query.GroupId);
        }
    }
}
