using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.SupervisorTaskOverviewGet
{
    /// <summary>US-8.2: SupervisorEmployeeId is always the caller's own, resolved server-side.</summary>
    public class SupervisorTaskOverviewGetQuery : IRequest<SupervisorTaskOverviewResponse>
    {
        public long SupervisorEmployeeId { get; set; }

        public SupervisorTaskOverviewGetQuery(long supervisorEmployeeId)
        {
            SupervisorEmployeeId = supervisorEmployeeId;
        }
    }
}
