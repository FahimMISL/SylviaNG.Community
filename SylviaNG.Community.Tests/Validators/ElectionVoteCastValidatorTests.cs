using FluentAssertions;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Tests.Validators;

public class ElectionVoteCastValidatorTests
{
    private readonly ElectionVoteCastValidator _validator = new();

    [Fact]
    public void Validate_WithValidSingleCandidate_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new ElectionVoteCastCommand(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCandidateIds_ShouldHaveError()
    {
        // Arrange
        var command = new ElectionVoteCastCommand(1, new ElectionVoteCastRequest { CandidateIds = new List<long>() }, voterId: 5);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.CandidateIds");
    }

    [Fact]
    public void Validate_WithDuplicateCandidateIds_ShouldHaveError()
    {
        // Arrange
        var command = new ElectionVoteCastCommand(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10, 10 } }, voterId: 5);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithZeroCandidateId_ShouldHaveError()
    {
        // Arrange
        var command = new ElectionVoteCastCommand(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 0 } }, voterId: 5);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNoVoterId_ShouldHaveError()
    {
        // Arrange
        var command = new ElectionVoteCastCommand(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 0);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "VoterId");
    }
}
