using FluentValidation;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageForward
{
    public class ChatMessageForwardValidator : AbstractValidator<ChatMessageForwardCommand>
    {
        public ChatMessageForwardValidator()
        {
            RuleFor(x => x.Request.ConversationIds)
                .NotEmpty().WithMessage("Choose at least one conversation to forward to.");
        }
    }
}
