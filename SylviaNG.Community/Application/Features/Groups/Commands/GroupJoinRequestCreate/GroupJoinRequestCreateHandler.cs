using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestCreate
{
    public class GroupJoinRequestCreateHandler : IRequestHandler<GroupJoinRequestCreateCommand, long>
    {
        private readonly IGroupService _groupService;

        public GroupJoinRequestCreateHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<long> Handle(GroupJoinRequestCreateCommand command, CancellationToken cancellationToken)
        {
            return await _groupService.RequestToJoinAsync(command.GroupId, command.CallerEmployeeId);
        }
    }
}
