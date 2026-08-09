using FluentValidation;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollCreate
{
    public class PollCreateValidator : AbstractValidator<PollCreateCommand>
    {
        public PollCreateValidator()
        {
            RuleFor(x => x.Request.Options)
                .NotNull().WithMessage("Options are required.")
                .Must(o => o.Count >= 2).WithMessage("A poll requires at least two options.");

            RuleForEach(x => x.Request.Options)
                .NotEmpty().WithMessage("Option text cannot be empty.")
                .MaximumLength(500).WithMessage("Option text must not exceed 500 characters.");
        }
    }
}
