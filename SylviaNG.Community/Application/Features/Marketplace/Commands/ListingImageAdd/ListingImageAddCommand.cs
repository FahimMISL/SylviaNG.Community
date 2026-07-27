using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageAdd
{
    public class ListingImageAddCommand : IRequest<long>
    {
        public long ListingId { get; set; }
        public ListingImageAddRequest Request { get; set; }

        public ListingImageAddCommand(long listingId, ListingImageAddRequest request)
        {
            ListingId = listingId;
            Request = request;
        }
    }
}
