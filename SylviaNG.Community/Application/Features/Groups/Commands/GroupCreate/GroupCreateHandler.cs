using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupCreate
{
    public class GroupCreateHandler : IRequestHandler<GroupCreateCommand, long>
    {
        private readonly IGroupService _groupService;

        public GroupCreateHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<long> Handle(GroupCreateCommand command, CancellationToken cancellationToken)
        {
            return await _groupService.CreateAsync(command.Request, command.CallerEmployeeId);
        }
    }
}
