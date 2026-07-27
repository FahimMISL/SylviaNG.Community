using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingImageGetAll
{
    public class ListingImageGetAllQuery : IRequest<List<ListingImageResponse>>
    {
        public long ListingId { get; set; }

        public ListingImageGetAllQuery(long listingId)
        {
            ListingId = listingId;
        }
    }
}
