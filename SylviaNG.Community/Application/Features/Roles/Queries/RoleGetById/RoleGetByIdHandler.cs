using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Roles.Queries.RoleGetById
{
    public class RoleGetByIdHandler : IRequestHandler<RoleGetByIdQuery, RoleResponse>
    {
        private readonly IRoleService _roleService;

        public RoleGetByIdHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<RoleResponse> Handle(RoleGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetByIdAsync(query.RoleId);
        }
    }
}
