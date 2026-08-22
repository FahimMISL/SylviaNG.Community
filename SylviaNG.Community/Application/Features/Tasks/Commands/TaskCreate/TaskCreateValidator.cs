using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate
{
    public class TaskCreateValidator : AbstractValidator<TaskCreateCommand>
    {
        public TaskCreateValidator()
        {
            // TeamId is optional (US-7.6: individual tasks have no team), but if provided must be valid.
            RuleFor(x => x.Request.TeamId)
                .GreaterThan(0).WithMessage("TeamId must be a positive id when provided.")
                .When(x => x.Request.TeamId.HasValue);

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

            // US-7.9: 1-14 days when provided; unset defaults to 2 in TaskMapper.ToEntity.
            RuleFor(x => x.Request.ReminderDays)
                .InclusiveBetween(1, 14).WithMessage("ReminderDays must be between 1 and 14.")
                .When(x => x.Request.ReminderDays.HasValue);
        }
    }
}
