using MediatR;
using SylviaNG.Community.Application.Features.EmployeeInterests.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Queries.EmployeeInterestGetAll
{
    public class EmployeeInterestGetAllHandler : IRequestHandler<EmployeeInterestGetAllQuery, List<EmployeeInterestResponse>>
    {
        private readonly IInterestService _interestService;

        public EmployeeInterestGetAllHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<List<EmployeeInterestResponse>> Handle(EmployeeInterestGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _interestService.GetEmployeeInterestsAsync(query.EmployeeId);
        }
    }
}
