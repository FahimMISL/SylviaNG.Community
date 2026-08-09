using FluentValidation;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillAssign
{
    public class EmployeeSkillAssignValidator : AbstractValidator<EmployeeSkillAssignCommand>
    {
        public EmployeeSkillAssignValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.SkillId)
                .GreaterThan(0).WithMessage("SkillId is required.");
        }
    }
}
