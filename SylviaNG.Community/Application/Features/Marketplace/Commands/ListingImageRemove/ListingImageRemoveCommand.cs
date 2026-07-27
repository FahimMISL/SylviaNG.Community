using MediatR;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageRemove
{
    public class ListingImageRemoveCommand : IRequest
    {
        public long ListingId { get; set; }
        public long ImageId { get; set; }

        public ListingImageRemoveCommand(long listingId, long imageId)
        {
            ListingId = listingId;
            ImageId = imageId;
        }
    }
}
