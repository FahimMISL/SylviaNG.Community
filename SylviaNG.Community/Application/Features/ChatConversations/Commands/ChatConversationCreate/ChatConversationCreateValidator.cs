using FluentValidation;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationCreate
{
    public class ChatConversationCreateValidator : AbstractValidator<ChatConversationCreateCommand>
    {
        public ChatConversationCreateValidator()
        {
            RuleFor(x => x.Request.ParticipantEmployeeIds)
                .NotEmpty().WithMessage("At least one other participant is required.");

            RuleFor(x => x.Request.ParticipantEmployeeIds)
                .Must(ids => ids.Count(id => id != 0) == 1)
                .When(x => x.Request.Type == ConversationTypeEnum.Direct && x.Request.ParticipantEmployeeIds.Count > 0)
                .WithMessage("A direct conversation needs exactly one other participant.");

            RuleFor(x => x.Request.ParticipantEmployeeIds)
                .Must(ids => ids.Distinct().Count() >= 2)
                .When(x => x.Request.Type == ConversationTypeEnum.Group && x.Request.ParticipantEmployeeIds.Count > 0)
                .WithMessage("A group conversation needs at least two other participants.");

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("A group conversation needs a name.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Type == ConversationTypeEnum.Group);
        }
    }
}
