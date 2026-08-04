using FluentAssertions;
using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;

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
    public void ToResponse_ShouldIncludeCoverPhotoUrl()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, CoverPhotoUrl = "uploads/employee-cover/2026-07/guid.jpg" };

        // Act
        var response = entity.ToResponse(viewerEmployeeId: 1, viewerIsHrAdmin: false, new CoreBatchLookupResult());

        // Assert
        response.CoverPhotoUrl.Should().Be("uploads/employee-cover/2026-07/guid.jpg");
    }
}
