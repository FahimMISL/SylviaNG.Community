using FluentAssertions;
using FluentValidation;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Tests.Validators;

public class AnnouncementCreateValidatorTests
{
    private readonly AnnouncementCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AnnouncementCreateCommand(new AnnouncementCreateRequest
        {
            Title = "Software Engineer",
            SiteId = 1,
            NumberOfPositions = 2,
            PostingDate = new DateTime(2025, 1, 1),
            ClosingDate = new DateTime(2025, 6, 30)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new AnnouncementCreateCommand(new AnnouncementCreateRequest
        {
            Title = "",
            SiteId = 1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithZeroSiteId_ShouldHaveError()
    {
        // Arrange
        var command = new AnnouncementCreateCommand(new AnnouncementCreateRequest
        {
            Title = "Valid Title",
            SiteId = 0
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.SiteId");
    }

    [Fact]
    public void Validate_WithMinSalaryGreaterThanMaxSalary_ShouldHaveError()
    {
        // Arrange
        var command = new AnnouncementCreateCommand(new AnnouncementCreateRequest
        {
            Title = "Valid Title",
            SiteId = 1,
            MinSalary = 100000,
            MaxSalary = 50000
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.MinSalary");
    }
}
