using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleDelete
{
    public class RoleDeleteHandler : IRequestHandler<RoleDeleteCommand>
    {
        private readonly IRoleService _roleService;

        public RoleDeleteHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task Handle(RoleDeleteCommand command, CancellationToken cancellationToken)
        {
            await _roleService.DeleteAsync(command.RoleId);
        }
    }
}
