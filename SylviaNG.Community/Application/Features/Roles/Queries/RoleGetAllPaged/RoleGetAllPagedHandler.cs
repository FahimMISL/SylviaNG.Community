using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Roles.Queries.RoleGetAllPaged
{
    public class RoleGetAllPagedHandler : IRequestHandler<RoleGetAllPagedQuery, PagedResult<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public RoleGetAllPagedHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<PagedResult<RoleResponse>> Handle(RoleGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetPaginatedAsync(query.Request);
        }
    }
}
