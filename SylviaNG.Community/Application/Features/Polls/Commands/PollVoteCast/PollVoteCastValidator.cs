using FluentValidation;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollVoteCast
{
    public class PollVoteCastValidator : AbstractValidator<PollVoteCastCommand>
    {
        public PollVoteCastValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.PollOptionId)
                .GreaterThan(0).WithMessage("PollOptionId is required.");
        }
    }
}
