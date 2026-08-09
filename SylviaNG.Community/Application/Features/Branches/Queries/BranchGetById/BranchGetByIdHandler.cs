using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Branches.Queries.BranchGetById
{
    public class BranchGetByIdHandler : IRequestHandler<BranchGetByIdQuery, BranchResponse>
    {
        private readonly IBranchService _branchService;

        public BranchGetByIdHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<BranchResponse> Handle(BranchGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _branchService.GetByIdAsync(query.BranchId);
        }
    }
}
