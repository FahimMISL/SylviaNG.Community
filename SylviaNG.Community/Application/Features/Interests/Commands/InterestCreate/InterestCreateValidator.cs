using FluentValidation;

namespace SylviaNG.Community.Application.Features.Interests.Commands.InterestCreate
{
    public class InterestCreateValidator : AbstractValidator<InterestCreateCommand>
    {
        public InterestCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
