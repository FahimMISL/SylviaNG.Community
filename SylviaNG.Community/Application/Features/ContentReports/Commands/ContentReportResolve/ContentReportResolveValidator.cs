using FluentValidation;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve
{
    public class ContentReportResolveValidator : AbstractValidator<ContentReportResolveCommand>
    {
        public ContentReportResolveValidator()
        {
            RuleFor(x => x.Request.ReviewedBy)
                .GreaterThan(0).WithMessage("ReviewedBy is required.");

            RuleFor(x => x.Request.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.");
        }
    }
}
