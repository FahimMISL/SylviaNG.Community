using FluentValidation;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate
{
    public class BranchCreateValidator : AbstractValidator<BranchCreateCommand>
    {
        public BranchCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Request.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");
        }
    }
}
