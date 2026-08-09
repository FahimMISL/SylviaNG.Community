using MediatR;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchDelete
{
    public class BranchDeleteCommand : IRequest
    {
        public long BranchId { get; set; }

        public BranchDeleteCommand(long branchId)
        {
            BranchId = branchId;
        }
    }
}
