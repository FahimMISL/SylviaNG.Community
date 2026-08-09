using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingUpdate
{
    public class ListingUpdateValidator : AbstractValidator<ListingUpdateCommand>
    {
        public ListingUpdateValidator()
        {
            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must not be negative.")
                .When(x => x.Request.Price.HasValue);
        }
    }
}
