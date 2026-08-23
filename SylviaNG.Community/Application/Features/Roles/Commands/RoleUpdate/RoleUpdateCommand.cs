using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateCommand : IRequest
    {
        public long RoleId { get; set; }
        public RoleUpdateRequest Request { get; set; }

        public RoleUpdateCommand(long roleId, RoleUpdateRequest request)
        {
            RoleId = roleId;
            Request = request;
        }
    }
}
