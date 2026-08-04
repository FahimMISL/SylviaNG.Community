using FluentValidation;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto
{
    public class EmployeeUpdatePhotoValidator : AbstractValidator<EmployeeUpdatePhotoCommand>
    {
        public EmployeeUpdatePhotoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.StoragePath)
                .NotEmpty().WithMessage("StoragePath is required.")
                .MaximumLength(500).WithMessage("StoragePath must not exceed 500 characters.")
                .Must(p => p.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                .WithMessage("StoragePath must be a value returned by the file-upload endpoint.");
        }
    }
}
