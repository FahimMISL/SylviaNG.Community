using FluentValidation;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskUpdate
{
    public class RecurringTaskUpdateValidator : AbstractValidator<RecurringTaskUpdateCommand>
    {
        public RecurringTaskUpdateValidator()
        {
            RuleFor(x => x.Request.Frequency)
                .MaximumLength(50).WithMessage("Frequency must not exceed 50 characters.")
                .When(x => x.Request.Frequency != null);

            RuleFor(x => x.Request.IntervalValue)
                .GreaterThan(0).WithMessage("IntervalValue must be greater than 0.")
                .When(x => x.Request.IntervalValue.HasValue);
        }
    }
}
