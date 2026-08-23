using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;

namespace SylviaNG.Community.Application.Features.Branches.Queries.BranchGetById
{
    public class BranchGetByIdQuery : IRequest<BranchResponse>
    {
        public long BranchId { get; set; }

        public BranchGetByIdQuery(long branchId)
        {
            BranchId = branchId;
        }
    }
}
