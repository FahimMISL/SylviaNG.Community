using MediatR;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Queries.DashboardPreferenceGetAllByEmployee
{
    public class DashboardPreferenceGetAllByEmployeeQuery : IRequest<List<DashboardPreferenceResponse>>
    {
        public long EmployeeId { get; set; }

        public DashboardPreferenceGetAllByEmployeeQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
