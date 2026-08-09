using FluentValidation;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportCreate
{
    public class ContentReportCreateValidator : AbstractValidator<ContentReportCreateCommand>
    {
        public ContentReportCreateValidator()
        {
            RuleFor(x => x.Request.ReportedBy)
                .GreaterThan(0).WithMessage("ReportedBy is required.");

            RuleFor(x => x.Request.PostId)
                .GreaterThan(0).WithMessage("PostId is required.");

            RuleFor(x => x.Request.Reason)
                .NotEmpty().WithMessage("Reason is required.");
        }
    }
}
