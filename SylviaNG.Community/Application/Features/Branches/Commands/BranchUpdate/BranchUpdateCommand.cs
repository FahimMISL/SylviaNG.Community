using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchUpdate
{
    public class BranchUpdateCommand : IRequest
    {
        public long BranchId { get; set; }
        public BranchUpdateRequest Request { get; set; }

        public BranchUpdateCommand(long branchId, BranchUpdateRequest request)
        {
            BranchId = branchId;
            Request = request;
        }
    }
}
