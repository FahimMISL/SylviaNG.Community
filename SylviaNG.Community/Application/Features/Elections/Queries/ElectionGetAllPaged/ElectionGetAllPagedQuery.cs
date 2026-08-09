using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetAllPaged
{
    public class ElectionGetAllPagedQuery : IRequest<PagedResult<ElectionResponse>>
    {
        public PagedRequest Request { get; set; }

        public ElectionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
