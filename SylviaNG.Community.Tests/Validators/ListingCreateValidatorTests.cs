using FluentAssertions;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Tests.Validators;

public class ListingCreateValidatorTests
{
    private readonly ListingCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ListingCreateCommand(10, false, new ListingCreateRequest
        {
            ListingType = "Item",
            Title = "Old bicycle",
            Category = "Sports",
            Price = 50,
            Currency = "USD"
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        var command = new ListingCreateCommand(10, false, new ListingCreateRequest
        {
            ListingType = "Item",
            Title = "",
            Category = "Sports",
            Price = 50,
            Currency = "USD"
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldHaveError()
    {
        var command = new ListingCreateCommand(10, false, new ListingCreateRequest
        {
            ListingType = "Item",
            Title = "Old bicycle",
            Category = "Sports",
            Price = -5,
            Currency = "USD"
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Price");
    }

    [Fact]
    public void Validate_WithZeroSellerId_ShouldHaveError()
    {
        var command = new ListingCreateCommand(0, false, new ListingCreateRequest
        {
            ListingType = "Item",
            Title = "Old bicycle",
            Category = "Sports",
            Price = 50,
            Currency = "USD"
        });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SellerId");
    }
}
