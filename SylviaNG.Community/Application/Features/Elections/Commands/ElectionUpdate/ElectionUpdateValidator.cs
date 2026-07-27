using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionUpdate
{
    public class ElectionUpdateValidator : AbstractValidator<ElectionUpdateCommand>
    {
        public ElectionUpdateValidator()
        {
            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.MaxSelection)
                .GreaterThanOrEqualTo(x => x.Request.MinSelection ?? 1)
                .When(x => x.Request.MaxSelection.HasValue)
                .WithMessage("MaxSelection must be greater than or equal to MinSelection.");

            RuleFor(x => x.Request.EndDate)
                .GreaterThan(x => x.Request.StartDate)
                .When(x => x.Request.EndDate.HasValue && x.Request.StartDate.HasValue)
                .WithMessage("EndDate must be after StartDate.");
        }
    }
}
