using FluentAssertions;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionAdd;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Tests.Validators;

public class SurveyQuestionAddValidatorTests
{
    private readonly SurveyQuestionAddValidator _validator = new();

    [Fact]
    public void Validate_WithUnknownQuestionType_ShouldFail()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "How satisfied are you?",
            QuestionType = "Foo"
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.QuestionType");
    }

    [Fact]
    public void Validate_WithSingleChoiceAndNoOptions_ShouldFail()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "Pick one",
            QuestionType = "SingleChoice",
            Options = new List<SurveyOptionCreateRequest>()
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Options");
    }

    [Fact]
    public void Validate_WithDuplicateOptionText_ShouldFail()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "Pick one",
            QuestionType = "SingleChoice",
            Options = new List<SurveyOptionCreateRequest>
            {
                new() { OptionText = "Yes" },
                new() { OptionText = " yes " }
            }
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Options");
    }

    [Fact]
    public void Validate_WithTextTypeAndNoOptions_ShouldSucceed()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "Any comments?",
            QuestionType = "Text",
            Options = new List<SurveyOptionCreateRequest>()
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithRatingTypeAndNoOptions_ShouldSucceed()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "Rate your experience",
            QuestionType = "Rating",
            Options = new List<SurveyOptionCreateRequest>()
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidSingleChoiceQuestion_ShouldSucceed()
    {
        var command = new SurveyQuestionAddCommand(1, new SurveyQuestionCreateRequest
        {
            QuestionText = "Pick one",
            QuestionType = "SingleChoice",
            Options = new List<SurveyOptionCreateRequest>
            {
                new() { OptionText = "Yes" },
                new() { OptionText = "No" }
            }
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
