using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetMy
{
    public class GroupGetMyHandler : IRequestHandler<GroupGetMyQuery, List<GroupResponse>>
    {
        private readonly IGroupService _groupService;

        public GroupGetMyHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<List<GroupResponse>> Handle(GroupGetMyQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetMyGroupsAsync(query.EmployeeId);
        }
    }
}
