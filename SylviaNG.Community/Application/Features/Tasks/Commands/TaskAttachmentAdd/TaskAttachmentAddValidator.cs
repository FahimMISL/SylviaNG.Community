using FluentValidation;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentAdd
{
    public class TaskAttachmentAddValidator : AbstractValidator<TaskAttachmentAddCommand>
    {
        public TaskAttachmentAddValidator()
        {
            RuleFor(x => x.Request.FileName)
                .NotEmpty().WithMessage("FileName is required.")
                .MaximumLength(255).WithMessage("FileName must not exceed 255 characters.");

            RuleFor(x => x.Request.FilePath)
                .NotEmpty().WithMessage("FilePath is required.")
                .MaximumLength(1000).WithMessage("FilePath must not exceed 1000 characters.");

            RuleFor(x => x.Request.FileSize)
                .GreaterThan(0).WithMessage("FileSize must be greater than 0.");

            RuleFor(x => x.Request.UploadedBy)
                .GreaterThan(0).WithMessage("UploadedBy is required.");
        }
    }
}
