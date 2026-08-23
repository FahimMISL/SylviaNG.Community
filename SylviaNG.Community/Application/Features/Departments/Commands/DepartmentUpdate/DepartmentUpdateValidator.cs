using FluentValidation;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateValidator : AbstractValidator<DepartmentUpdateCommand>
    {
        public DepartmentUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);

            RuleFor(x => x.Request.Code)
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.")
                .When(x => x.Request.Code != null);
        }
    }
}
