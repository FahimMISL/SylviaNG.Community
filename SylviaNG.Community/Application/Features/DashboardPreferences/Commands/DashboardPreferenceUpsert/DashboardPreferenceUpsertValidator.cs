using FluentValidation;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert
{
    public class DashboardPreferenceUpsertValidator : AbstractValidator<DashboardPreferenceUpsertCommand>
    {
        public DashboardPreferenceUpsertValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.WidgetName)
                .NotEmpty().WithMessage("WidgetName is required.")
                .MaximumLength(100).WithMessage("WidgetName must not exceed 100 characters.");

            RuleFor(x => x.Request.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must not be negative.");
        }
    }
}
