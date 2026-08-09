using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate
{
    public class ElectionCreateValidator : AbstractValidator<ElectionCreateCommand>
    {
        public ElectionCreateValidator()
        {
            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.ElectionType)
                .NotEmpty().WithMessage("ElectionType is required.")
                .MaximumLength(50).WithMessage("ElectionType must not exceed 50 characters.");

            RuleFor(x => x.Request.CandidateType)
                .NotEmpty().WithMessage("CandidateType is required.")
                .MaximumLength(50).WithMessage("CandidateType must not exceed 50 characters.");

            RuleFor(x => x.Request.AudienceScope)
                .NotEmpty().WithMessage("AudienceScope is required.")
                .MaximumLength(50).WithMessage("AudienceScope must not exceed 50 characters.");

            RuleFor(x => x.Request.MinSelection)
                .GreaterThan(0).WithMessage("MinSelection must be greater than 0.");

            RuleFor(x => x.Request.MaxSelection)
                .GreaterThanOrEqualTo(x => x.Request.MinSelection)
                .WithMessage("MaxSelection must be greater than or equal to MinSelection.");

            RuleFor(x => x.Request.EndDate)
                .GreaterThan(x => x.Request.StartDate)
                .When(x => x.Request.EndDate.HasValue)
                .WithMessage("EndDate must be after StartDate.");
        }
    }
}
