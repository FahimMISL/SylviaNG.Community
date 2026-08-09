using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate
{
    public class RoleCreateHandler : IRequestHandler<RoleCreateCommand, long>
    {
        private readonly IRoleService _roleService;

        public RoleCreateHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<long> Handle(RoleCreateCommand command, CancellationToken cancellationToken)
        {
            return await _roleService.CreateAsync(command.Request);
        }
    }
}
