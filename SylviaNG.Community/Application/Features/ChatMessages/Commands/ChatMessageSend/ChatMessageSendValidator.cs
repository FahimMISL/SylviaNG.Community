using FluentValidation;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSend
{
    public class ChatMessageSendValidator : AbstractValidator<ChatMessageSendCommand>
    {
        public ChatMessageSendValidator()
        {
            RuleFor(x => x.Request.Body)
                .NotEmpty().WithMessage("Message body is required.")
                .MaximumLength(4000).WithMessage("Message must not exceed 4000 characters.")
                .When(x => x.Request.MessageType == MessageTypeEnum.Text);

            RuleFor(x => x.Request.Attachments)
                .NotEmpty().WithMessage("At least one image or file is required.")
                .When(x => x.Request.MessageType == MessageTypeEnum.Attachment);

            RuleFor(x => x.Request.Attachments)
                .Must(a => a.All(item => item.AttachmentType != ChatAttachmentTypeEnum.Voice))
                .When(x => x.Request.MessageType == MessageTypeEnum.Attachment && x.Request.Attachments.Count > 0)
                .WithMessage("Voice notes must be sent as their own message.");

            RuleFor(x => x.Request.Attachments)
                .Must(a => a.Count == 1 && a[0].AttachmentType == ChatAttachmentTypeEnum.Voice)
                .When(x => x.Request.MessageType == MessageTypeEnum.Voice)
                .WithMessage("A voice message needs exactly one voice attachment.");
        }
    }
}
