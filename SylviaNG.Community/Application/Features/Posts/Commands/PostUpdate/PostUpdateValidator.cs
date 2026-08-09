using FluentValidation;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate
{
    public class PostUpdateValidator : AbstractValidator<PostUpdateCommand>
    {
        public PostUpdateValidator()
        {
            RuleFor(x => x.Request.Type)
                .MaximumLength(50).WithMessage("Type must not exceed 50 characters.")
                .When(x => x.Request.Type != null);

            RuleFor(x => x.Request.Visibility!.Value)
                .IsInEnum().WithMessage("Visibility must be a valid value.")
                .When(x => x.Request.Visibility.HasValue);
        }
    }
}
