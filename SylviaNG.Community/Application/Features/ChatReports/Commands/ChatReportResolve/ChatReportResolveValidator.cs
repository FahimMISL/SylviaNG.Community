using FluentValidation;

namespace SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve
{
    public class ChatReportResolveValidator : AbstractValidator<ChatReportResolveCommand>
    {
        public ChatReportResolveValidator()
        {
            RuleFor(x => x.Request.ReviewedBy)
                .GreaterThan(0).WithMessage("ReviewedBy is required.");

            RuleFor(x => x.Request.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.");
        }
    }
}
