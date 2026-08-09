using FluentAssertions;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Tests.Validators;

public class SurveyAudienceAddValidatorTests
{
    private readonly SurveyAudienceAddValidator _validator = new();

    [Fact]
    public void Validate_WithDepartmentTypeAndNoDepartmentId_ShouldFail()
    {
        var command = new SurveyAudienceAddCommand(1, new SurveyAudienceCreateRequest { AudienceType = "Department" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.DepartmentId");
    }

    [Fact]
    public void Validate_WithBranchTypeAndNoBranchId_ShouldFail()
    {
        var command = new SurveyAudienceAddCommand(1, new SurveyAudienceCreateRequest { AudienceType = "Branch" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.BranchId");
    }

    [Fact]
    public void Validate_WithUnknownAudienceType_ShouldFail()
    {
        var command = new SurveyAudienceAddCommand(1, new SurveyAudienceCreateRequest { AudienceType = "Foo" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.AudienceType");
    }

    [Fact]
    public void Validate_WithEntireCompanyAndNoDepartmentOrBranchId_ShouldSucceed()
    {
        var command = new SurveyAudienceAddCommand(1, new SurveyAudienceCreateRequest { AudienceType = "EntireCompany" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDepartmentTypeAndDepartmentId_ShouldSucceed()
    {
        var command = new SurveyAudienceAddCommand(1, new SurveyAudienceCreateRequest { AudienceType = "Department", DepartmentId = 2 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
