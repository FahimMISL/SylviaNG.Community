using FluentValidation;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationAddParticipants
{
    public class ChatConversationAddParticipantsValidator : AbstractValidator<ChatConversationAddParticipantsCommand>
    {
        public ChatConversationAddParticipantsValidator()
        {
            RuleFor(x => x.Request.EmployeeIds)
                .NotEmpty().WithMessage("Choose at least one person to add.");
        }
    }
}
