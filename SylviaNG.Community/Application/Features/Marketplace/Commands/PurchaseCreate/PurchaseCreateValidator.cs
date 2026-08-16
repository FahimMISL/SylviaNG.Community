using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.PurchaseCreate
{
    public class PurchaseCreateValidator : AbstractValidator<PurchaseCreateCommand>
    {
        public PurchaseCreateValidator()
        {
            RuleFor(x => x.BuyerId)
                .GreaterThan(0).WithMessage("BuyerId is required.");

            RuleFor(x => x.Request.ListingId)
                .GreaterThan(0).WithMessage("ListingId is required.");

            RuleFor(x => x.Request.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
