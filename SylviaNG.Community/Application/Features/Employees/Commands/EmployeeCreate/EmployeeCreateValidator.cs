using FluentValidation;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate
{
    public class EmployeeCreateValidator : AbstractValidator<EmployeeCreateCommand>
    {
        public EmployeeCreateValidator()
        {
            RuleFor(x => x.Request.EmployeeName)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

            RuleFor(x => x.Request.DesignationId)
                .GreaterThan(0).WithMessage("Designation is required.");

            RuleFor(x => x.Request.DepartmentId)
                .GreaterThan(0).WithMessage("Department is required.");

            RuleFor(x => x.Request.SiteId)
                .GreaterThan(0).WithMessage("Branch is required.");

            RuleFor(x => x.Request.DateOfJoining)
                .LessThanOrEqualTo(DateTimeUtility.TodayLocal()).WithMessage("Date of joining cannot be in the future.");

            RuleFor(x => x.Request.DateOfBirth)
                .LessThanOrEqualTo(DateTimeUtility.TodayLocal().AddYears(-13)).WithMessage("You must be at least 13 years old.")
                .GreaterThanOrEqualTo(DateTimeUtility.TodayLocal().AddYears(-100)).WithMessage("Please enter a valid date of birth.")
                .When(x => x.Request.DateOfBirth.HasValue);
        }
    }
}
