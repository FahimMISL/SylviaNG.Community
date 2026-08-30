using FluentValidation;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationUpdateGroup
{
    public class ChatConversationUpdateGroupValidator : AbstractValidator<ChatConversationUpdateGroupCommand>
    {
        public ChatConversationUpdateGroupValidator()
        {
            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Group name cannot be empty.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);
        }
    }
}
