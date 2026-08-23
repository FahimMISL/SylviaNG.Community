using FluentValidation;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate
{
    public class DepartmentCreateValidator : AbstractValidator<DepartmentCreateCommand>
    {
        public DepartmentCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Code)
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
        }
    }
}
