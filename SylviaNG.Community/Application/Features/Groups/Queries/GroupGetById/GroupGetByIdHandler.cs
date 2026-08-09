using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetById
{
    public class GroupGetByIdHandler : IRequestHandler<GroupGetByIdQuery, GroupResponse>
    {
        private readonly IGroupService _groupService;

        public GroupGetByIdHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<GroupResponse> Handle(GroupGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _groupService.GetByIdAsync(query.GroupId);
        }
    }
}
