using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate
{
    public class ListingCreateCommand : IRequest<long>
    {
        public long SellerId { get; set; }
        public bool IsHrOrAdmin { get; set; }
        public ListingCreateRequest Request { get; set; }

        public ListingCreateCommand(long sellerId, bool isHrOrAdmin, ListingCreateRequest request)
        {
            SellerId = sellerId;
            IsHrOrAdmin = isHrOrAdmin;
            Request = request;
        }
    }
}
