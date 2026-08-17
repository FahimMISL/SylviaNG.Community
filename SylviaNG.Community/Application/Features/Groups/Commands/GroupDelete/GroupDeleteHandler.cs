using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupDelete
{
    public class GroupDeleteHandler : IRequestHandler<GroupDeleteCommand>
    {
        private readonly IGroupService _groupService;

        public GroupDeleteHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(GroupDeleteCommand command, CancellationToken cancellationToken)
        {
            await _groupService.DeleteAsync(command.GroupId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
