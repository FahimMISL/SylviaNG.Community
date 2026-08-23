using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd
{
    public class SurveyAudienceAddValidator : AbstractValidator<SurveyAudienceAddCommand>
    {
        public SurveyAudienceAddValidator()
        {
            RuleFor(x => x.Request.AudienceType)
                .NotEmpty().WithMessage("AudienceType is required.")
                .MaximumLength(50).WithMessage("AudienceType must not exceed 50 characters.")
                .Must(t => SurveyAudienceTypes.All.Contains(t))
                .WithMessage($"AudienceType must be one of: {string.Join(", ", SurveyAudienceTypes.All)}.");

            RuleFor(x => x.Request.DepartmentId)
                .NotNull().WithMessage("DepartmentId is required when AudienceType is Department.")
                .When(x => x.Request.AudienceType == SurveyAudienceTypes.Department);

            RuleFor(x => x.Request.BranchId)
                .NotNull().WithMessage("BranchId is required when AudienceType is Branch.")
                .When(x => x.Request.AudienceType == SurveyAudienceTypes.Branch);
        }
    }
}
