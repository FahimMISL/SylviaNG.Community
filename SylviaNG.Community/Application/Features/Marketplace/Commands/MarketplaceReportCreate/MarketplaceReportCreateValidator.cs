using FluentValidation;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportCreate
{
    public class MarketplaceReportCreateValidator : AbstractValidator<MarketplaceReportCreateCommand>
    {
        public MarketplaceReportCreateValidator()
        {
            RuleFor(x => x.ReportedBy)
                .GreaterThan(0).WithMessage("ReportedBy is required.");

            RuleFor(x => x.Request.ListingId)
                .GreaterThan(0).WithMessage("ListingId is required.");

            RuleFor(x => x.Request.Reason)
                .NotEmpty().WithMessage("Reason is required.");
        }
    }
}
