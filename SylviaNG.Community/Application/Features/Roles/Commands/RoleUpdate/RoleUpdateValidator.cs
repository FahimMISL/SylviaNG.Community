using FluentValidation;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateValidator : AbstractValidator<RoleUpdateCommand>
    {
        public RoleUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);
        }
    }
}
