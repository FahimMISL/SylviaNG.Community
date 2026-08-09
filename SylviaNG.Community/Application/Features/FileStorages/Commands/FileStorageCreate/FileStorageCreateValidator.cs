using FluentValidation;

namespace SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate
{
    public class FileStorageCreateValidator : AbstractValidator<FileStorageCreateCommand>
    {
        public FileStorageCreateValidator()
        {
            RuleFor(x => x.Request.Module)
                .NotEmpty().WithMessage("Module is required.")
                .MaximumLength(100).WithMessage("Module must not exceed 100 characters.");

            RuleFor(x => x.Request.FileName)
                .NotEmpty().WithMessage("FileName is required.")
                .MaximumLength(255).WithMessage("FileName must not exceed 255 characters.");

            RuleFor(x => x.Request.OriginalFileName)
                .NotEmpty().WithMessage("OriginalFileName is required.")
                .MaximumLength(255).WithMessage("OriginalFileName must not exceed 255 characters.");

            RuleFor(x => x.Request.StoragePath)
                .NotEmpty().WithMessage("StoragePath is required.")
                .MaximumLength(500).WithMessage("StoragePath must not exceed 500 characters.");

            RuleFor(x => x.Request.FileSize)
                .GreaterThanOrEqualTo(0).WithMessage("FileSize must not be negative.");

            RuleFor(x => x.Request.UploadedBy)
                .GreaterThan(0).WithMessage("UploadedBy is required.");
        }
    }
}
