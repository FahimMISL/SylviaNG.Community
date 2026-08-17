using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Branches.Queries.BranchGetAllPaged
{
    public class BranchGetAllPagedQuery : IRequest<PagedResult<BranchResponse>>
    {
        public PagedRequest Request { get; set; }

        public BranchGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
