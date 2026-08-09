using FluentValidation;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate
{
    public class DesignationCreateValidator : AbstractValidator<DesignationCreateCommand>
    {
        public DesignationCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Grade)
                .MaximumLength(50).WithMessage("Grade must not exceed 50 characters.");
        }
    }
}
