using FluentValidation;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestAssign
{
    public class EmployeeInterestAssignValidator : AbstractValidator<EmployeeInterestAssignCommand>
    {
        public EmployeeInterestAssignValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.InterestId)
                .GreaterThan(0).WithMessage("InterestId is required.");
        }
    }
}
