using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.AdminDashboardSummaryGet
{
    /// <summary>US-8.3: company-wide summary - no parameters, HRAdminOnly is enforced at the controller.</summary>
    public class AdminDashboardSummaryGetQuery : IRequest<AdminDashboardSummaryResponse>
    {
    }
}
