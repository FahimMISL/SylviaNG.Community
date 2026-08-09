using MediatR;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteRemove
{
    public class FavoriteRemoveCommand : IRequest
    {
        public long EmployeeId { get; set; }
        public long ListingId { get; set; }

        public FavoriteRemoveCommand(long employeeId, long listingId)
        {
            EmployeeId = employeeId;
            ListingId = listingId;
        }
    }
}
