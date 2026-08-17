using FluentValidation;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRoleChange
{
    public class GroupMemberRoleChangeValidator : AbstractValidator<GroupMemberRoleChangeCommand>
    {
        public GroupMemberRoleChangeValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.NewRole)
                .IsInEnum().WithMessage("NewRole must be a valid value.");
        }
    }
}
