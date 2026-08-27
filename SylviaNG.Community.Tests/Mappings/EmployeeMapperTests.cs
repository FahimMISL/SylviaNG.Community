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

    [Fact]
    public void ToEntity_ShouldMapDateOfJoining()
    {
        // Arrange
        var request = new EmployeeCreateRequest { EmployeeName = "Ayesha", Email = "a@example.com", DateOfJoining = new DateTime(2026, 3, 15) };

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.DateOfJoining.Should().Be(new DateOnly(2026, 3, 15));
    }

    [Fact]
    public void ApplyProfileUpdate_ShouldSetDateOfBirth()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, DateOfBirth = null };
        var request = new EmployeeUpdateProfileRequest { DateOfBirth = new DateTime(1995, 6, 20) };

        // Act
        entity.ApplyProfileUpdate(request);

        // Assert
        entity.DateOfBirth.Should().Be(new DateOnly(1995, 6, 20));
    }

    [Fact]
    public void ApplyProfileUpdate_WithNullDateOfBirth_ShouldClearIt()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, DateOfBirth = new DateOnly(1995, 6, 20) };
        var request = new EmployeeUpdateProfileRequest { DateOfBirth = null };

        // Act
        entity.ApplyProfileUpdate(request);

        // Assert
        entity.DateOfBirth.Should().BeNull();
    }

    [Fact]
    public void ToResponse_WhenViewerIsNotOwnerOrHrAdmin_ShouldHideDateOfBirth()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, DateOfBirth = new DateOnly(1995, 6, 20) };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 2, viewerIsHrAdmin: false, new CoreBatchLookupResult(), new List<EmployeeContactLink>());

        // Assert
        response.DateOfBirth.Should().BeNull();
    }

    [Fact]
    public void ToResponse_WhenViewerIsOwner_ShouldShowDateOfBirth()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, DateOfBirth = new DateOnly(1995, 6, 20) };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 1, viewerIsHrAdmin: false, new CoreBatchLookupResult(), new List<EmployeeContactLink>());

        // Assert
        response.DateOfBirth.Should().Be(new DateOnly(1995, 6, 20));
    }

    [Fact]
    public void ToResponse_WhenViewerIsHrAdmin_ShouldShowDateOfBirth()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, DateOfBirth = new DateOnly(1995, 6, 20) };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 99, viewerIsHrAdmin: true, new CoreBatchLookupResult(), new List<EmployeeContactLink>());

        // Assert
        response.DateOfBirth.Should().Be(new DateOnly(1995, 6, 20));
    }

    [Fact]
    public void ToTodayEventResponse_ShouldMapBirthdayFields()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, EmployeeName = "Ayesha Rahman", PhotoUrl = "photo.jpg" };

        // Act
        var response = entity.ToTodayEventResponse(TodayEventTypeEnum.Birthday);

        // Assert
        response.EmployeeId.Should().Be(1);
        response.EmployeeName.Should().Be("Ayesha Rahman");
        response.PhotoUrl.Should().Be("photo.jpg");
        response.EventType.Should().Be(TodayEventTypeEnum.Birthday);
        response.YearsOfService.Should().BeNull();
    }

    [Fact]
    public void ToTodayEventResponse_ShouldMapAnniversaryFieldsWithYearsOfService()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, EmployeeName = "Tanvir Hasan" };

        // Act
        var response = entity.ToTodayEventResponse(TodayEventTypeEnum.Anniversary, yearsOfService: 3);

        // Assert
        response.EventType.Should().Be(TodayEventTypeEnum.Anniversary);
        response.YearsOfService.Should().Be(3);
    }

    [Fact]
    public void ToNewJoineeResponse_ShouldMapFields()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, EmployeeName = "Farhana Akter", PhotoUrl = "photo.jpg", DateOfJoining = new DateOnly(2026, 8, 20) };

        // Act
        var response = entity.ToNewJoineeResponse();

        // Assert
        response.EmployeeId.Should().Be(1);
        response.EmployeeName.Should().Be("Farhana Akter");
        response.PhotoUrl.Should().Be("photo.jpg");
        response.DateOfJoining.Should().Be(new DateOnly(2026, 8, 20));
    }
}
