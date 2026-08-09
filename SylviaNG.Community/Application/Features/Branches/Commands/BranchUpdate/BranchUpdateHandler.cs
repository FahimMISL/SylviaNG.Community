using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchUpdate
{
    public class BranchUpdateHandler : IRequestHandler<BranchUpdateCommand>
    {
        private readonly IBranchService _branchService;

        public BranchUpdateHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task Handle(BranchUpdateCommand command, CancellationToken cancellationToken)
        {
            await _branchService.UpdateAsync(command.BranchId, command.Request);
        }
    }
}
