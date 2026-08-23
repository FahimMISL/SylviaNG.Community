using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchDelete
{
    public class BranchDeleteHandler : IRequestHandler<BranchDeleteCommand>
    {
        private readonly IBranchService _branchService;

        public BranchDeleteHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task Handle(BranchDeleteCommand command, CancellationToken cancellationToken)
        {
            await _branchService.DeleteAsync(command.BranchId);
        }
    }
}
