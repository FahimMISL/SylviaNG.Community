using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagAssign
{
    public class TaskTagAssignValidator : AbstractValidator<TaskTagAssignCommand>
    {
        public TaskTagAssignValidator()
        {
            RuleFor(x => x.Request.TagId)
                .GreaterThan(0).WithMessage("TagId is required.");
        }
    }
}
