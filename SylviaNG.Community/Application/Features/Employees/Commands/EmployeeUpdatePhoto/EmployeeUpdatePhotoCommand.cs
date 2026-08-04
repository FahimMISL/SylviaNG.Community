using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto
{
    public class EmployeeUpdatePhotoCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }
        public EmployeeUpdatePhotoRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long? ViewerEmployeeId { get; set; }

        public EmployeeUpdatePhotoCommand(long employeeId, EmployeeUpdatePhotoRequest request, long? viewerEmployeeId)
        {
            EmployeeId = employeeId;
            Request = request;
            ViewerEmployeeId = viewerEmployeeId;
        }
    }
}
