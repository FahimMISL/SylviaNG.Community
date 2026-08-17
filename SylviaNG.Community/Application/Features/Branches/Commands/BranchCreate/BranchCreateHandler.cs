using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate
{
    public class BranchCreateHandler : IRequestHandler<BranchCreateCommand, long>
    {
        private readonly IBranchService _branchService;

        public BranchCreateHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<long> Handle(BranchCreateCommand command, CancellationToken cancellationToken)
        {
            return await _branchService.CreateAsync(command.Request);
        }
    }
}
