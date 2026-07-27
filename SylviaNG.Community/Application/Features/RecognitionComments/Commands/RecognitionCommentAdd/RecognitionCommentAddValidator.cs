using FluentValidation;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd
{
    public class RecognitionCommentAddValidator : AbstractValidator<RecognitionCommentAddCommand>
    {
        public RecognitionCommentAddValidator()
        {
            RuleFor(x => x.RecognitionId)
                .GreaterThan(0).WithMessage("RecognitionId is required.");

            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Comment)
                .NotEmpty().WithMessage("Comment is required.");
        }
    }
}
