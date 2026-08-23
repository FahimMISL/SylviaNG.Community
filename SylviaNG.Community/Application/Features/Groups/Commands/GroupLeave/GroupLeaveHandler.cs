using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupLeave
{
    public class GroupLeaveHandler : IRequestHandler<GroupLeaveCommand>
    {
        private readonly IGroupService _groupService;

        public GroupLeaveHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupLeaveCommand command, CancellationToken cancellationToken)
        {
            await _groupService.LeaveAsync(command.GroupId, command.CallerEmployeeId);
        }
    }
}
