using FluentValidation;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReport
{
    public class ChatMessageReportValidator : AbstractValidator<ChatMessageReportCommand>
    {
        public ChatMessageReportValidator()
        {
            RuleFor(x => x.Request.Reason)
                .NotEmpty().WithMessage("Please describe why you're reporting this message.")
                .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters.");
        }
    }
}
