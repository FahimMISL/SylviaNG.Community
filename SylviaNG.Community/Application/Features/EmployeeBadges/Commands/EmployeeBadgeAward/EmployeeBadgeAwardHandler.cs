using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeBadges.Commands.EmployeeBadgeAward
{
    public class EmployeeBadgeAwardHandler : IRequestHandler<EmployeeBadgeAwardCommand, long>
    {
        private readonly IBadgeService _badgeService;

        public EmployeeBadgeAwardHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<long> Handle(EmployeeBadgeAwardCommand command, CancellationToken cancellationToken)
        {
            return await _badgeService.AwardToEmployeeAsync(command.EmployeeId, command.Request);
        }
    }
}
