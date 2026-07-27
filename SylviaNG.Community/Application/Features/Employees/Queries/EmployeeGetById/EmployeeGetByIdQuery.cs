using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetById
{
    public class EmployeeGetByIdQuery : IRequest<EmployeeResponse>
    {
        public long EmployeeId { get; set; }

        /// <summary>Both populated by the controller from ICurrentUserService - never from client input.</summary>
        public long? ViewerEmployeeId { get; set; }
        public bool ViewerIsHrAdmin { get; set; }

        public EmployeeGetByIdQuery(long employeeId, long? viewerEmployeeId, bool viewerIsHrAdmin)
        {
            EmployeeId = employeeId;
            ViewerEmployeeId = viewerEmployeeId;
            ViewerIsHrAdmin = viewerIsHrAdmin;
        }
    }
}
