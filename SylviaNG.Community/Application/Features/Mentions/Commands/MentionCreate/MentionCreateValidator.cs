using FluentValidation;

namespace SylviaNG.Community.Application.Features.Mentions.Commands.MentionCreate
{
    public class MentionCreateValidator : AbstractValidator<MentionCreateCommand>
    {
        public MentionCreateValidator()
        {
            RuleFor(x => x.Request.MentionedEmployeeId)
                .GreaterThan(0).WithMessage("MentionedEmployeeId is required.");

            RuleFor(x => x.Request.MentionedBy)
                .GreaterThan(0).WithMessage("MentionedBy is required.");

            RuleFor(x => x.Request.EntityType)
                .NotEmpty().WithMessage("EntityType is required.")
                .MaximumLength(50).WithMessage("EntityType must not exceed 50 characters.");

            RuleFor(x => x.Request.EntityId)
                .GreaterThan(0).WithMessage("EntityId is required.");
        }
    }
}
