using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate
{
    public class RoleCreateCommand : IRequest<long>
    {
        public RoleCreateRequest Request { get; set; }

        public RoleCreateCommand(RoleCreateRequest request)
        {
            Request = request;
        }
    }
}
