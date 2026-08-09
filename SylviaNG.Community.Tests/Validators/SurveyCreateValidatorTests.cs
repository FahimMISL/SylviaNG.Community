using FluentAssertions;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Tests.Validators;

public class SurveyCreateValidatorTests
{
    private readonly SurveyCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "Pulse" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest { Title = "", SurveyType = "Pulse" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithEmptySurveyType_ShouldHaveError()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.SurveyType");
    }

    [Fact]
    public void Validate_WithTitleTooLong_ShouldHaveError()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest { Title = new string('a', 201), SurveyType = "Pulse" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithValidExternalUrl_ShouldHaveNoErrors()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest
        {
            Title = "External Pulse",
            SurveyType = "Pulse",
            ExternalUrl = "https://forms.google.com/abc123",
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMalformedExternalUrl_ShouldHaveError()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest
        {
            Title = "External Pulse",
            SurveyType = "Pulse",
            ExternalUrl = "not-a-url",
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ExternalUrl");
    }

    [Fact]
    public void Validate_WithNullExternalUrl_ShouldHaveNoErrors()
    {
        var command = new SurveyCreateCommand(new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "Pulse", ExternalUrl = null });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
