using FluentAssertions;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Tests.Validators;

public class ElectionCreateValidatorTests
{
    private readonly ElectionCreateValidator _validator = new();

    private static ElectionCreateRequest ValidRequest() => new()
    {
        Title = "Employee of the Year",
        ElectionType = "SingleChoice",
        CandidateType = "Employee",
        AudienceScope = "All",
        MinSelection = 1,
        MaxSelection = 1,
        StartDate = DateTime.UtcNow
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new ElectionCreateCommand(ValidRequest(), createdBy: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.Title = "";
        var command = new ElectionCreateCommand(request, createdBy: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithMaxSelectionLessThanMinSelection_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.MinSelection = 3;
        request.MaxSelection = 1;
        var command = new ElectionCreateCommand(request, createdBy: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MaxSelection");
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.StartDate = DateTime.UtcNow;
        request.EndDate = DateTime.UtcNow.AddDays(-1);
        var command = new ElectionCreateCommand(request, createdBy: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.EndDate");
    }
}
