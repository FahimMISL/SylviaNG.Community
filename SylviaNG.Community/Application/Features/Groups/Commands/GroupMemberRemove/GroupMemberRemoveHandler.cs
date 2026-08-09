using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRemove
{
    public class GroupMemberRemoveHandler : IRequestHandler<GroupMemberRemoveCommand>
    {
        private readonly IGroupService _groupService;

        public GroupMemberRemoveHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupMemberRemoveCommand command, CancellationToken cancellationToken)
        {
            await _groupService.RemoveMemberAsync(command.GroupId, command.EmployeeId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
