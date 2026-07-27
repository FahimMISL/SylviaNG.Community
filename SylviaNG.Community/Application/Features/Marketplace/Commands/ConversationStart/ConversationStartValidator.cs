using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ConversationStart
{
    public class ConversationStartValidator : AbstractValidator<ConversationStartCommand>
    {
        public ConversationStartValidator()
        {
            RuleFor(x => x.InitiatorEmployeeId)
                .GreaterThan(0).WithMessage("InitiatorEmployeeId is required.");

            RuleFor(x => x.Request.ListingId)
                .GreaterThan(0).WithMessage("ListingId is required.");
        }
    }
}
