using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportResolve
{
    public class MarketplaceReportResolveValidator : AbstractValidator<MarketplaceReportResolveCommand>
    {
        public MarketplaceReportResolveValidator()
        {
            RuleFor(x => x.Request.Status)
                .NotEmpty().WithMessage("Status is required.");
        }
    }
}
