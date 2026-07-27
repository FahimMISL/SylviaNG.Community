using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingReject
{
    public class ListingRejectCommand : IRequest<Unit>
    {
        public long ListingId { get; set; }
        public long ApproverId { get; set; }
        public ListingRejectRequest Request { get; set; }

        public ListingRejectCommand(long listingId, long approverId, ListingRejectRequest request)
        {
            ListingId = listingId;
            ApproverId = approverId;
            Request = request;
        }
    }
}
