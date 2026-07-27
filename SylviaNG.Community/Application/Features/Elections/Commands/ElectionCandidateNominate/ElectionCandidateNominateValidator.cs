using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominate
{
    public class ElectionCandidateNominateValidator : AbstractValidator<ElectionCandidateNominateCommand>
    {
        public ElectionCandidateNominateValidator()
        {
            RuleFor(x => x.Request.CandidateType)
                .NotEmpty().WithMessage("CandidateType is required.")
                .MaximumLength(50).WithMessage("CandidateType must not exceed 50 characters.");

            RuleFor(x => x.Request)
                .Must(r => (r.EmployeeId.HasValue && !r.TeamId.HasValue) || (!r.EmployeeId.HasValue && r.TeamId.HasValue))
                .WithMessage("Exactly one of EmployeeId or TeamId must be set.");
        }
    }
}
