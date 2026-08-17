using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestApprove
{
    public class GroupJoinRequestApproveHandler : IRequestHandler<GroupJoinRequestApproveCommand>
    {
        private readonly IGroupService _groupService;

        public GroupJoinRequestApproveHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupJoinRequestApproveCommand command, CancellationToken cancellationToken)
        {
            await _groupService.ApproveJoinRequestAsync(command.GroupJoinRequestId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
