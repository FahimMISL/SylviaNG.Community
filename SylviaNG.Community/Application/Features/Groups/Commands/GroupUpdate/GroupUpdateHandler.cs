using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupUpdate
{
    public class GroupUpdateHandler : IRequestHandler<GroupUpdateCommand>
    {
        private readonly IGroupService _groupService;

        public GroupUpdateHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupUpdateCommand command, CancellationToken cancellationToken)
        {
            await _groupService.UpdateAsync(command.GroupId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
