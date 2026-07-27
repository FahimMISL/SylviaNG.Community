using FluentValidation;

namespace SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate
{
    public class SkillCreateValidator : AbstractValidator<SkillCreateCommand>
    {
        public SkillCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
