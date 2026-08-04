using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateCoverPhoto
{
    public class EmployeeUpdateCoverPhotoCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }
        public EmployeeUpdateCoverPhotoRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long? ViewerEmployeeId { get; set; }

        public EmployeeUpdateCoverPhotoCommand(long employeeId, EmployeeUpdateCoverPhotoRequest request, long? viewerEmployeeId)
        {
            EmployeeId = employeeId;
            Request = request;
            ViewerEmployeeId = viewerEmployeeId;
        }
    }
}
