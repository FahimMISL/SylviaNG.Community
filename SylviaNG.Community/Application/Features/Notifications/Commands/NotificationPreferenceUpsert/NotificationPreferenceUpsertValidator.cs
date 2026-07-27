using FluentValidation;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert
{
    public class NotificationPreferenceUpsertValidator : AbstractValidator<NotificationPreferenceUpsertCommand>
    {
        public NotificationPreferenceUpsertValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");
        }
    }
}
