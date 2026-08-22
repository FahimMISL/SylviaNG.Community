using FluentAssertions;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Tests.Validators;

public class SurveyResponseSubmitValidatorTests
{
    private readonly SurveyResponseSubmitValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new SurveyResponseSubmitCommand(1, new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest>
            {
                new() { QuestionId = 1, OptionId = 10 }
            }
        }, 5);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNoAnswers_ShouldHaveNoErrors()
    {
        // Empty Answers is the "Mark as Completed" submission for an external survey (no
        // CES-native questions to answer) - this must stay valid, not rejected.
        var command = new SurveyResponseSubmitCommand(1, new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest>()
        }, 5);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAnswerMissingOptionAndText_ShouldHaveError()
    {
        var command = new SurveyResponseSubmitCommand(1, new SurveySubmissionRequest
        {
            Answers = new List<SurveyAnswerSubmitRequest> { new() { QuestionId = 1 } }
        }, 5);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
