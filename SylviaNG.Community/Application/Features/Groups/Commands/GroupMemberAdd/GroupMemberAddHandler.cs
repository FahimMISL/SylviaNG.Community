using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberAdd
{
    public class GroupMemberAddHandler : IRequestHandler<GroupMemberAddCommand, long>
    {
        private readonly IGroupService _groupService;

        public GroupMemberAddHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<long> Handle(GroupMemberAddCommand command, CancellationToken cancellationToken)
        {
            return await _groupService.AddMemberAsync(command.GroupId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
