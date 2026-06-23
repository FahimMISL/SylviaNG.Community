using FluentValidation;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementUpdate
{
    public class AnnouncementUpdateValidator : AbstractValidator<AnnouncementUpdateCommand>
    {
        public AnnouncementUpdateValidator()
        {
            RuleFor(x => x.AnnouncementId)
                .GreaterThan(0).WithMessage("AnnouncementId is required.");

            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.MinSalary)
                .LessThan(x => x.Request.MaxSalary)
                .When(x => x.Request.MinSalary.HasValue && x.Request.MaxSalary.HasValue)
                .WithMessage("MinSalary must be less than MaxSalary.");
        }
    }
}
