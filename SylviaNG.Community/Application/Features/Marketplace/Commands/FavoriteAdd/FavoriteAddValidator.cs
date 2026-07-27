using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteAdd
{
    public class FavoriteAddValidator : AbstractValidator<FavoriteAddCommand>
    {
        public FavoriteAddValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.ListingId)
                .GreaterThan(0).WithMessage("ListingId is required.");
        }
    }
}
