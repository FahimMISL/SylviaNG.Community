using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingUpdate
{
    public class ListingUpdateCommand : IRequest
    {
        public long ListingId { get; set; }
        public ListingUpdateRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public ListingUpdateCommand(long listingId, ListingUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            ListingId = listingId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
