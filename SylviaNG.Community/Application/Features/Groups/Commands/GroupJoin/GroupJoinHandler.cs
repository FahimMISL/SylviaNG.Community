using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoin
{
    public class GroupJoinHandler : IRequestHandler<GroupJoinCommand>
    {
        private readonly IGroupService _groupService;

        public GroupJoinHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupJoinCommand command, CancellationToken cancellationToken)
        {
            await _groupService.JoinAsync(command.GroupId, command.CallerEmployeeId);
        }
    }
}
