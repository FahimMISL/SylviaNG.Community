using MediatR;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceDelete
{
    public class DashboardPreferenceDeleteCommand : IRequest
    {
        public long PreferenceId { get; set; }

        public DashboardPreferenceDeleteCommand(long preferenceId)
        {
            PreferenceId = preferenceId;
        }
    }
}
