using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd
{
    public class TaskCommentAddValidator : AbstractValidator<TaskCommentAddCommand>
    {
        public TaskCommentAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Comment)
                .NotEmpty().WithMessage("Comment is required.");
        }
    }
}
