using FluentValidation;

namespace SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateCommand>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
