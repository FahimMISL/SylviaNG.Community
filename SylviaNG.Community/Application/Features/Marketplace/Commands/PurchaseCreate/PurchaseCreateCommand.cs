using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.PurchaseCreate
{
    public class PurchaseCreateCommand : IRequest<long>
    {
        public long BuyerId { get; set; }
        public PurchaseCreateRequest Request { get; set; }

        public PurchaseCreateCommand(long buyerId, PurchaseCreateRequest request)
        {
            BuyerId = buyerId;
            Request = request;
        }
    }
}
