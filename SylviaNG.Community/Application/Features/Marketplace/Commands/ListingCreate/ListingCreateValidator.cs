using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate
{
    public class ListingCreateValidator : AbstractValidator<ListingCreateCommand>
    {
        public ListingCreateValidator()
        {
            RuleFor(x => x.SellerId)
                .GreaterThan(0).WithMessage("SellerId is required.");

            RuleFor(x => x.Request.ListingType)
                .NotEmpty().WithMessage("ListingType is required.")
                .MaximumLength(50).WithMessage("ListingType must not exceed 50 characters.");

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

            RuleFor(x => x.Request.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must not be negative.");

            RuleFor(x => x.Request.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .MaximumLength(10).WithMessage("Currency must not exceed 10 characters.");
        }
    }
}
