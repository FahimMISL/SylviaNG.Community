using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate
{
    public class TaskCreateValidator : AbstractValidator<TaskCreateCommand>
    {
        public TaskCreateValidator()
        {
            RuleFor(x => x.Request.TeamId)
                .GreaterThan(0).WithMessage("TeamId is required.");

            RuleFor(x => x.Request.AssignedBy)
                .GreaterThan(0).WithMessage("AssignedBy is required.");

            RuleFor(x => x.Request.AssignedTo)
                .GreaterThan(0).WithMessage("AssignedTo is required.");

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .MaximumLength(50).WithMessage("Priority must not exceed 50 characters.");

            RuleFor(x => x.Request.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.");

            RuleFor(x => x.Request.ReminderDays)
                .GreaterThanOrEqualTo(0).WithMessage("ReminderDays cannot be negative.")
                .When(x => x.Request.ReminderDays.HasValue);
        }
    }
}
