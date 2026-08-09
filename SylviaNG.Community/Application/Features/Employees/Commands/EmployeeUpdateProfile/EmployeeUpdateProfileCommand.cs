using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile
{
    public class EmployeeUpdateProfileCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }
        public EmployeeUpdateProfileRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long? ViewerEmployeeId { get; set; }

        public EmployeeUpdateProfileCommand(long employeeId, EmployeeUpdateProfileRequest request, long? viewerEmployeeId)
        {
            EmployeeId = employeeId;
            Request = request;
            ViewerEmployeeId = viewerEmployeeId;
        }
    }
}
