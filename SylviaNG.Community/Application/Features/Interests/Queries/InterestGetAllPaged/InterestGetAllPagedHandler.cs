using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAllPaged
{
    public class InterestGetAllPagedHandler : IRequestHandler<InterestGetAllPagedQuery, PagedResult<InterestResponse>>
    {
        private readonly IInterestService _interestService;

        public InterestGetAllPagedHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<PagedResult<InterestResponse>> Handle(InterestGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interestService.GetPaginatedAsync(query.Request);
        }
    }
}
