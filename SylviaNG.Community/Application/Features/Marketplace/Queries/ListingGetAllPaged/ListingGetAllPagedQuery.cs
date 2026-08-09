using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetAllPaged
{
    public class ListingGetAllPagedQuery : IRequest<PagedResult<ListingResponse>>
    {
        public ListingFilterRequest Request { get; set; }

        public ListingGetAllPagedQuery(ListingFilterRequest request)
        {
            Request = request;
        }
    }
}
