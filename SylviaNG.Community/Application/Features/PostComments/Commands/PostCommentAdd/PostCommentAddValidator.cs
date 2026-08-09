using FluentValidation;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd
{
    public class PostCommentAddValidator : AbstractValidator<PostCommentAddCommand>
    {
        public PostCommentAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Content)
                .NotEmpty().WithMessage("Content is required.");
        }
    }
}
