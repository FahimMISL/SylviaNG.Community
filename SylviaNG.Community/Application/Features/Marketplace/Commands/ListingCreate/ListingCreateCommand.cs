using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate
{
    public class ListingCreateCommand : IRequest<long>
    {
        public long SellerId { get; set; }
        public ListingCreateRequest Request { get; set; }

        public ListingCreateCommand(long sellerId, ListingCreateRequest request)
        {
            SellerId = sellerId;
            Request = request;
        }
    }
}
