using FluentValidation;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberAdd
{
    public class GroupMemberAddValidator : AbstractValidator<GroupMemberAddCommand>
    {
        public GroupMemberAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");
        }
    }
}
