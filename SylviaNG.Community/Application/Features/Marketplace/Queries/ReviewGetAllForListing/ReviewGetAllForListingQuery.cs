using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewGetAllForListing
{
    public class ReviewGetAllForListingQuery : IRequest<List<ReviewResponse>>
    {
        public long ListingId { get; set; }

        public ReviewGetAllForListingQuery(long listingId)
        {
            ListingId = listingId;
        }
    }
}
