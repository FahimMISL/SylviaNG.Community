using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Branches.Queries.BranchGetAllPaged
{
    public class BranchGetAllPagedHandler : IRequestHandler<BranchGetAllPagedQuery, PagedResult<BranchResponse>>
    {
        private readonly IBranchService _branchService;

        public BranchGetAllPagedHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<PagedResult<BranchResponse>> Handle(BranchGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _branchService.GetPaginatedAsync(query.Request);
        }
    }
}
