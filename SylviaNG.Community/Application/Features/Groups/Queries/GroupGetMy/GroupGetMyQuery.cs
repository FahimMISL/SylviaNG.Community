using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetMy
{
    public class GroupGetMyQuery : IRequest<List<GroupResponse>>
    {
        public long EmployeeId { get; set; }

        public GroupGetMyQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
