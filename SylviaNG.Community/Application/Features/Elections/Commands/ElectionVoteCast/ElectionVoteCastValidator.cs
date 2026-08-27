using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast
{
    public class ElectionVoteCastValidator : AbstractValidator<ElectionVoteCastCommand>
    {
        public ElectionVoteCastValidator()
        {
            RuleFor(x => x.Request.CandidateIds)
                .NotEmpty().WithMessage("At least one candidate must be selected.");

            RuleForEach(x => x.Request.CandidateIds)
                .GreaterThan(0).WithMessage("CandidateId is required.");

            RuleFor(x => x.Request.CandidateIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .When(x => x.Request.CandidateIds.Count > 0)
                .WithMessage("Duplicate candidate selections are not allowed.");

            RuleFor(x => x.VoterId)
                .GreaterThan(0).WithMessage("VoterId is required.");
        }
    }
}
