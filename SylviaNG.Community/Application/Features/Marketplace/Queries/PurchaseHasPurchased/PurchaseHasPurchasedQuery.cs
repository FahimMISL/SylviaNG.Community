using MediatR;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseHasPurchased
{
    public class PurchaseHasPurchasedQuery : IRequest<bool>
    {
        public long EmployeeId { get; set; }
        public long ListingId { get; set; }

        public PurchaseHasPurchasedQuery(long employeeId, long listingId)
        {
            EmployeeId = employeeId;
            ListingId = listingId;
        }
    }
}
