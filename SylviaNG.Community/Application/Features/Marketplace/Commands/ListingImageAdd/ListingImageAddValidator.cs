using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageAdd
{
    public class ListingImageAddValidator : AbstractValidator<ListingImageAddCommand>
    {
        public ListingImageAddValidator()
        {
            RuleFor(x => x.Request.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.");
        }
    }
}
