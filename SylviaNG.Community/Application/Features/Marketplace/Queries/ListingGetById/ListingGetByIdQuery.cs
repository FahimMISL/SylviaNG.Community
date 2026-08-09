using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetById
{
    public class ListingGetByIdQuery : IRequest<ListingResponse>
    {
        public long ListingId { get; set; }

        public ListingGetByIdQuery(long listingId)
        {
            ListingId = listingId;
        }
    }
}
