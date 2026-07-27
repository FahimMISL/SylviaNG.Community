using FluentValidation;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate
{
    public class RecurringTaskCreateValidator : AbstractValidator<RecurringTaskCreateCommand>
    {
        public RecurringTaskCreateValidator()
        {
            RuleFor(x => x.Request.Frequency)
                .NotEmpty().WithMessage("Frequency is required.")
                .MaximumLength(50).WithMessage("Frequency must not exceed 50 characters.");

            RuleFor(x => x.Request.IntervalValue)
                .GreaterThan(0).WithMessage("IntervalValue must be greater than 0.");

            RuleFor(x => x.Request.EndDate)
                .GreaterThanOrEqualTo(x => x.Request.StartDate)
                .WithMessage("EndDate must be on or after StartDate.")
                .When(x => x.Request.EndDate.HasValue);
        }
    }
}
