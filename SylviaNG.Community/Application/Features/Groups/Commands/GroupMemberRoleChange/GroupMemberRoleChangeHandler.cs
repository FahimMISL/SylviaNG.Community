using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRoleChange
{
    public class GroupMemberRoleChangeHandler : IRequestHandler<GroupMemberRoleChangeCommand>
    {
        private readonly IGroupService _groupService;

        public GroupMemberRoleChangeHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupMemberRoleChangeCommand command, CancellationToken cancellationToken)
        {
            await _groupService.ChangeMemberRoleAsync(command.GroupId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
