using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewCreate
{
    public class ReviewCreateValidator : AbstractValidator<ReviewCreateCommand>
    {
        public ReviewCreateValidator()
        {
            RuleFor(x => x.ReviewerId)
                .GreaterThan(0).WithMessage("ReviewerId is required.");

            RuleFor(x => x.Request.ListingId)
                .GreaterThan(0).WithMessage("ListingId is required.");

            RuleFor(x => x.Request.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
