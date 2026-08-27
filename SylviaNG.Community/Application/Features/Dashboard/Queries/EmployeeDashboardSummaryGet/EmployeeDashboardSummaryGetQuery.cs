using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.EmployeeDashboardSummaryGet
{
    /// <summary>US-8.1: EmployeeId is always the caller's own, resolved server-side - never trusted from the client.</summary>
    public class EmployeeDashboardSummaryGetQuery : IRequest<EmployeeDashboardSummaryResponse>
    {
        public long EmployeeId { get; set; }

        public EmployeeDashboardSummaryGetQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
