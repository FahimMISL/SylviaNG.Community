using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MessageSend
{
    public class MessageSendValidator : AbstractValidator<MessageSendCommand>
    {
        public MessageSendValidator()
        {
            RuleFor(x => x.SenderId)
                .GreaterThan(0).WithMessage("SenderId is required.");

            RuleFor(x => x.Request.MessageText)
                .NotEmpty().WithMessage("MessageText is required.");
        }
    }
}
