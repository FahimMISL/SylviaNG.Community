using MediatR;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingDelete
{
    public class ListingDeleteCommand : IRequest
    {
        public long ListingId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public ListingDeleteCommand(long listingId, long callerEmployeeId, bool isHrOrAdmin)
        {
            ListingId = listingId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
