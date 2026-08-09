using FluentValidation;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile
{
    public class EmployeeUpdateProfileValidator : AbstractValidator<EmployeeUpdateProfileCommand>
    {
        public EmployeeUpdateProfileValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Bio)
                .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters.");

            RuleFor(x => x.Request.Skills)
                .MaximumLength(1000).WithMessage("Skills must not exceed 1000 characters.");

            RuleFor(x => x.Request.Interests)
                .MaximumLength(1000).WithMessage("Interests must not exceed 1000 characters.");

            RuleFor(x => x.Request.Achievements)
                .MaximumLength(1000).WithMessage("Achievements must not exceed 1000 characters.");

            RuleFor(x => x.Request.CommunityContributions)
                .MaximumLength(1000).WithMessage("Community contributions must not exceed 1000 characters.");

            RuleFor(x => x.Request.Phone)
                .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

            RuleFor(x => x.Request.Email)
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Request.Email));

            RuleFor(x => x.Request.Extension)
                .MaximumLength(20).WithMessage("Extension must not exceed 20 characters.");

            RuleFor(x => x.Request.PhoneVisibility).IsInEnum();
            RuleFor(x => x.Request.EmailVisibility).IsInEnum();
            RuleFor(x => x.Request.ExtensionVisibility).IsInEnum();

            RuleForEach(x => x.Request.ContactLinks).ChildRules(link =>
            {
                link.RuleFor(l => l.Platform)
                    .NotEmpty().WithMessage("Platform is required.")
                    .MaximumLength(50).WithMessage("Platform must not exceed 50 characters.");

                link.RuleFor(l => l.Url)
                    .NotEmpty().WithMessage("URL is required.")
                    .MaximumLength(500).WithMessage("URL must not exceed 500 characters.")
                    .Must(BeAValidUrl).WithMessage("URL must be a valid absolute URL.");

                link.RuleFor(l => l.Visibility).IsInEnum();
            });
        }

        private static bool BeAValidUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
