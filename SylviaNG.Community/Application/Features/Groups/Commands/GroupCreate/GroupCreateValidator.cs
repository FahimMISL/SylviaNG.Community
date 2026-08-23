using FluentValidation;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupCreate
{
    public class GroupCreateValidator : AbstractValidator<GroupCreateCommand>
    {
        public GroupCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Visibility)
                .IsInEnum().WithMessage("Visibility must be a valid value.");

            RuleFor(x => x.CallerEmployeeId)
                .GreaterThan(0).WithMessage("CallerEmployeeId is required.");
        }
    }
}
