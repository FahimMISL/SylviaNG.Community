using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingReject
{
    public class ListingRejectValidator : AbstractValidator<ListingRejectCommand>
    {
        public ListingRejectValidator()
        {
            RuleFor(x => x.Request.RejectionReason)
                .NotEmpty().WithMessage("RejectionReason is required.");
        }
    }
}
