using MediatR;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingApprove
{
    public class ListingApproveCommand : IRequest<Unit>
    {
        public long ListingId { get; set; }
        public long ApproverId { get; set; }

        public ListingApproveCommand(long listingId, long approverId)
        {
            ListingId = listingId;
            ApproverId = approverId;
        }
    }
}
