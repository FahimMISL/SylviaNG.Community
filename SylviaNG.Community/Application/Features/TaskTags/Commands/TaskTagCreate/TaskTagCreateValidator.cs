using FluentValidation;

namespace SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate
{
    public class TaskTagCreateValidator : AbstractValidator<TaskTagCreateCommand>
    {
        public TaskTagCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Request.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
