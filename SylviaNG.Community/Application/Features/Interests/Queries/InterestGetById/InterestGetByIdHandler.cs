using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetById
{
    public class InterestGetByIdHandler : IRequestHandler<InterestGetByIdQuery, InterestResponse>
    {
        private readonly IInterestService _interestService;

        public InterestGetByIdHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<InterestResponse> Handle(InterestGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _interestService.GetByIdAsync(query.InterestId);
        }
    }
}
