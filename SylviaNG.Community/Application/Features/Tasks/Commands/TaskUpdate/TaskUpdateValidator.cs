using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate
{
    public class TaskUpdateValidator : AbstractValidator<TaskUpdateCommand>
    {
        public TaskUpdateValidator()
        {
            RuleFor(x => x.Request.TeamId)
                .GreaterThan(0).WithMessage("TeamId must be a positive id when provided.")
                .When(x => x.Request.TeamId.HasValue);

            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.Priority)
                .MaximumLength(50).WithMessage("Priority must not exceed 50 characters.")
                .When(x => x.Request.Priority != null);

            RuleFor(x => x.Request.Status)
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.")
                .When(x => x.Request.Status != null);

            // US-7.9: 1-14 days when provided.
            RuleFor(x => x.Request.ReminderDays)
                .InclusiveBetween(1, 14).WithMessage("ReminderDays must be between 1 and 14.")
                .When(x => x.Request.ReminderDays.HasValue);
        }
    }
}
