using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkReassign
{
    public class TaskBulkReassignValidator : AbstractValidator<TaskBulkReassignCommand>
    {
        public TaskBulkReassignValidator()
        {
            RuleFor(x => x.Request.TaskIds)
                .NotEmpty().WithMessage("At least one TaskId is required.");

            RuleFor(x => x.Request.NewAssignedTo)
                .GreaterThan(0).WithMessage("NewAssignedTo is required.");
        }
    }
}
