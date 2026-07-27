using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast
{
    public class ElectionVoteCastValidator : AbstractValidator<ElectionVoteCastCommand>
    {
        public ElectionVoteCastValidator()
        {
            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");

            RuleFor(x => x.VoterId)
                .GreaterThan(0).WithMessage("VoterId is required.");
        }
    }
}
