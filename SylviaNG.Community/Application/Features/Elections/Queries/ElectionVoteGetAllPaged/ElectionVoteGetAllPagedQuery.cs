using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionVoteGetAllPaged
{
    public class ElectionVoteGetAllPagedQuery : IRequest<PagedResult<ElectionVoteResponse>>
    {
        public long ElectionId { get; set; }
        public PagedRequest Request { get; set; }

        public ElectionVoteGetAllPagedQuery(long electionId, PagedRequest request)
        {
            ElectionId = electionId;
            Request = request;
        }
    }
}
