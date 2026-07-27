using FluentValidation;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeCreate
{
    public class BadgeCreateValidator : AbstractValidator<BadgeCreateCommand>
    {
        public BadgeCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Icon)
                .MaximumLength(300).WithMessage("Icon must not exceed 300 characters.");
        }
    }
}
