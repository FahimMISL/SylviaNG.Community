using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateHandler : IRequestHandler<RoleUpdateCommand>
    {
        private readonly IRoleService _roleService;

        public RoleUpdateHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task Handle(RoleUpdateCommand command, CancellationToken cancellationToken)
        {
            await _roleService.UpdateAsync(command.RoleId, command.Request);
        }
    }
}
