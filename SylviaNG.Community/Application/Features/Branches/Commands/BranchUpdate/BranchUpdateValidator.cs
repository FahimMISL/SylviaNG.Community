using FluentValidation;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchUpdate
{
    public class BranchUpdateValidator : AbstractValidator<BranchUpdateCommand>
    {
        public BranchUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);

            RuleFor(x => x.Request.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
                .When(x => x.Request.City != null);

            RuleFor(x => x.Request.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
                .When(x => x.Request.Country != null);
        }
    }
}
