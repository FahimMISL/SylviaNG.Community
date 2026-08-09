using FluentValidation;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationUpdate
{
    public class DesignationUpdateValidator : AbstractValidator<DesignationUpdateCommand>
    {
        public DesignationUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);

            RuleFor(x => x.Request.Grade)
                .MaximumLength(50).WithMessage("Grade must not exceed 50 characters.")
                .When(x => x.Request.Grade != null);
        }
    }
}
