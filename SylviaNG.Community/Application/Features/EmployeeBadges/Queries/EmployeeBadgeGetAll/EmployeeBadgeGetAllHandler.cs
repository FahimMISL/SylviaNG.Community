using MediatR;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeBadges.Queries.EmployeeBadgeGetAll
{
    public class EmployeeBadgeGetAllHandler : IRequestHandler<EmployeeBadgeGetAllQuery, List<EmployeeBadgeResponse>>
    {
        private readonly IBadgeService _badgeService;

        public EmployeeBadgeGetAllHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<List<EmployeeBadgeResponse>> Handle(EmployeeBadgeGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _badgeService.GetEmployeeBadgesAsync(query.EmployeeId);
        }
    }
}
