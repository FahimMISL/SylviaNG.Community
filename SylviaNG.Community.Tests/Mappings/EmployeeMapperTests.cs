using FluentAssertions;
using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Tests.Mappings;

public class EmployeeMapperTests
{
    [Fact]
    public void ApplyProfileUpdate_ShouldSetAchievementsAndCommunityContributions()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, Achievements = "Old", CommunityContributions = "Old" };
        var request = new EmployeeUpdateProfileRequest
        {
            Achievements = "Employee of the month",
            CommunityContributions = "Mentored 3 interns"
        };

        // Act
        entity.ApplyProfileUpdate(request);

        // Assert
        entity.Achievements.Should().Be("Employee of the month");
        entity.CommunityContributions.Should().Be("Mentored 3 interns");
    }

    [Fact]
    public void ApplyProfileUpdate_ShouldSetPhoneEmailExtension()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, Phone = "old", Email = "old@example.com", Extension = "1000" };
        var request = new EmployeeUpdateProfileRequest
        {
            Phone = "+880-1710-999999",
            Email = "new@example.com",
            Extension = "2000"
        };

        // Act
        entity.ApplyProfileUpdate(request);

        // Assert
        entity.Phone.Should().Be("+880-1710-999999");
        entity.Email.Should().Be("new@example.com");
        entity.Extension.Should().Be("2000");
    }

    [Fact]
    public void ToResponse_ShouldIncludeCoverPhotoUrl()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, CoverPhotoUrl = "uploads/employee-cover/2026-07/guid.jpg" };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 1, viewerIsHrAdmin: false, new CoreBatchLookupResult(), new List<EmployeeContactLink>());

        // Assert
        response.CoverPhotoUrl.Should().Be("uploads/employee-cover/2026-07/guid.jpg");
    }

    [Fact]
    public void ToResponse_ShouldIncludeAllContactLinks_WhenOwner()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1 };
        var links = new List<EmployeeContactLink>
        {
            new() { EmployeeContactLinkId = 10, Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = ContactVisibilityEnum.Private },
            new() { EmployeeContactLinkId = 20, Platform = "GitHub", Url = "https://github.com/ayesha", Visibility = ContactVisibilityEnum.Public }
        };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 1, viewerIsHrAdmin: false, new CoreBatchLookupResult(), links);

        // Assert
        response.ContactLinks.Should().HaveCount(2);
    }

    [Fact]
    public void ToResponse_ShouldIncludeAllContactLinks_WhenHrAdmin()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1 };
        var links = new List<EmployeeContactLink>
        {
            new() { EmployeeContactLinkId = 10, Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = ContactVisibilityEnum.Private }
        };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 99, viewerIsHrAdmin: true, new CoreBatchLookupResult(), links);

        // Assert
        response.ContactLinks.Should().ContainSingle();
    }

    [Fact]
    public void ToResponse_ShouldOmitPrivateContactLinks_WhenThirdPartyViewer()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1 };
        var links = new List<EmployeeContactLink>
        {
            new() { EmployeeContactLinkId = 10, Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = ContactVisibilityEnum.Private },
            new() { EmployeeContactLinkId = 20, Platform = "GitHub", Url = "https://github.com/ayesha", Visibility = ContactVisibilityEnum.Public }
        };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 2, viewerIsHrAdmin: false, new CoreBatchLookupResult(), links);

        // Assert
        response.ContactLinks.Should().ContainSingle(l => l.Platform == "GitHub");
    }
}
