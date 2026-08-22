using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkCancel
{
    public class TaskBulkCancelValidator : AbstractValidator<TaskBulkCancelCommand>
    {
        public TaskBulkCancelValidator()
        {
            RuleFor(x => x.Request.TaskIds)
                .NotEmpty().WithMessage("At least one TaskId is required.");
        }
    }
}
