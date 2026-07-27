using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate
{
    public class TaskUpdateValidator : AbstractValidator<TaskUpdateCommand>
    {
        public TaskUpdateValidator()
        {
            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.Priority)
                .MaximumLength(50).WithMessage("Priority must not exceed 50 characters.")
                .When(x => x.Request.Priority != null);

            RuleFor(x => x.Request.Status)
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.")
                .When(x => x.Request.Status != null);

            RuleFor(x => x.Request.ReminderDays)
                .GreaterThanOrEqualTo(0).WithMessage("ReminderDays cannot be negative.")
                .When(x => x.Request.ReminderDays.HasValue);
        }
    }
}
