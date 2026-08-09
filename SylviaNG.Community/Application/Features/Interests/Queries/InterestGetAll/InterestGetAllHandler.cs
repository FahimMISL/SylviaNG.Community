using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAll
{
    public class InterestGetAllHandler : IRequestHandler<InterestGetAllQuery, List<InterestResponse>>
    {
        private readonly IInterestService _interestService;

        public InterestGetAllHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<List<InterestResponse>> Handle(InterestGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _interestService.GetAllAsync();
        }
    }
}
