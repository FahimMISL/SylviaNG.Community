using MediatR;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleDelete
{
    public class RoleDeleteCommand : IRequest
    {
        public long RoleId { get; set; }

        public RoleDeleteCommand(long roleId)
        {
            RoleId = roleId;
        }
    }
}
