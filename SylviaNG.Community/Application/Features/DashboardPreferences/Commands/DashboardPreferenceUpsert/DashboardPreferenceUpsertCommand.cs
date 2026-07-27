using MediatR;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert
{
    public class DashboardPreferenceUpsertCommand : IRequest<long>
    {
        public DashboardPreferenceUpsertRequest Request { get; set; }

        public DashboardPreferenceUpsertCommand(DashboardPreferenceUpsertRequest request)
        {
            Request = request;
        }
    }
}
