using FluentValidation;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentUpdate
{
    public class PostCommentUpdateValidator : AbstractValidator<PostCommentUpdateCommand>
    {
        public PostCommentUpdateValidator()
        {
            RuleFor(x => x.Request.Content)
                .NotEmpty().WithMessage("Content is required.");
        }
    }
}
