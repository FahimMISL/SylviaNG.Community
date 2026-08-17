using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;

namespace SylviaNG.Community.Application.Features.Roles.Queries.RoleGetById
{
    public class RoleGetByIdQuery : IRequest<RoleResponse>
    {
        public long RoleId { get; set; }

        public RoleGetByIdQuery(long roleId)
        {
            RoleId = roleId;
        }
    }
}
