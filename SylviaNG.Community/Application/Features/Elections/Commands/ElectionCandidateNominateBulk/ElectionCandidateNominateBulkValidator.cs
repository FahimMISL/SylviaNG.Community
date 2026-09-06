using FluentValidation;
using SylviaNG.Community.Domain.Constants;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominateBulk
{
    public class ElectionCandidateNominateBulkValidator : AbstractValidator<ElectionCandidateNominateBulkCommand>
    {
        private static readonly string[] ValidScopes =
        {
            ElectionAudienceScope.Organization,
            ElectionAudienceScope.Branch,
            ElectionAudienceScope.Department,
            ElectionAudienceScope.Team
        };

        public ElectionCandidateNominateBulkValidator()
        {
            RuleFor(x => x.Request.Scope)
                .NotEmpty().WithMessage("Scope is required.")
                .Must(scope => ValidScopes.Contains(scope))
                .WithMessage($"Scope must be one of: {string.Join(", ", ValidScopes)}.");

            RuleFor(x => x.Request.TargetIds)
                .NotEmpty()
                .When(x => x.Request.Scope != ElectionAudienceScope.Organization)
                .WithMessage("At least one target id is required unless Scope is Organization.");
        }
    }
}
