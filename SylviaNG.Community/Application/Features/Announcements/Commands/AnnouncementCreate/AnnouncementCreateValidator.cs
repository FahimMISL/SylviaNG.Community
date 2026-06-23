using FluentValidation;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate
{
    public class AnnouncementCreateValidator : AbstractValidator<AnnouncementCreateCommand>
    {
        public AnnouncementCreateValidator()
        {
            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.SiteId)
                .GreaterThan(0).WithMessage("SiteId is required.");

            RuleFor(x => x.Request.ExpiryDate)
                .GreaterThan(x => x.Request.PublishDate)
                .When(x => x.Request.PublishDate.HasValue && x.Request.ExpiryDate.HasValue)
                .WithMessage("Expiry date must be after publish date.");
        }
    }
}