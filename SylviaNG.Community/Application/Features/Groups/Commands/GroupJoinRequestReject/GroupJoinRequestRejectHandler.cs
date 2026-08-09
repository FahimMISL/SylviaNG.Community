using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestReject
{
    public class GroupJoinRequestRejectHandler : IRequestHandler<GroupJoinRequestRejectCommand>
    {
        private readonly IGroupService _groupService;

        public GroupJoinRequestRejectHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupJoinRequestRejectCommand command, CancellationToken cancellationToken)
        {
            await _groupService.RejectJoinRequestAsync(command.GroupJoinRequestId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
