using FluentValidation;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeUpdate
{
    public class BadgeUpdateValidator : AbstractValidator<BadgeUpdateCommand>
    {
        public BadgeUpdateValidator()
        {
            RuleFor(x => x.BadgeId)
                .GreaterThan(0).WithMessage("BadgeId is required.");

            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Icon)
                .MaximumLength(300).WithMessage("Icon must not exceed 300 characters.");
        }
    }
}
