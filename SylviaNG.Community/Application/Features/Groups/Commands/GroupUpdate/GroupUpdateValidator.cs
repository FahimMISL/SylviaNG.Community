using FluentValidation;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupUpdate
{
    public class GroupUpdateValidator : AbstractValidator<GroupUpdateCommand>
    {
        public GroupUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);

            RuleFor(x => x.Request.Visibility)
                .IsInEnum().WithMessage("Visibility must be a valid value.")
                .When(x => x.Request.Visibility.HasValue);
        }
    }
}
