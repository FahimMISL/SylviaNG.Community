using FluentValidation;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate
{
    public class NotificationCreateValidator : AbstractValidator<NotificationCreateCommand>
    {
        public NotificationCreateValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.Category)
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");
        }
    }
}
