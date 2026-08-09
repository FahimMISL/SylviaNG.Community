using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupCreate
{
    public class GroupCreateCommand : IRequest<long>
    {
        public GroupCreateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public GroupCreateCommand(GroupCreateRequest request, long callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
