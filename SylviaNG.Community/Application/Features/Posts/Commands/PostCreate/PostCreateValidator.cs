using FluentValidation;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostCreate
{
    public class PostCreateValidator : AbstractValidator<PostCreateCommand>
    {
        public PostCreateValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(50).WithMessage("Type must not exceed 50 characters.");

            RuleFor(x => x.Request.Visibility)
                .NotEmpty().WithMessage("Visibility is required.")
                .MaximumLength(50).WithMessage("Visibility must not exceed 50 characters.");
        }
    }
}
