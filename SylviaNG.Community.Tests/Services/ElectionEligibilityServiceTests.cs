using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ElectionEligibilityServiceTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;
    private readonly ElectionEligibilityService _service;

    public ElectionEligibilityServiceTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
        _service = new ElectionEligibilityService(_employeeRepositoryMock.Object, _teamMemberRepositoryMock.Object);
    }

    private static Election Election(string audienceScope) => new() { ElectionId = 1, Title = "T", AudienceScope = audienceScope };

    private static List<ElectionAudienceTarget> Targets(params string[] targetIds) =>
        targetIds.Select(id => new ElectionAudienceTarget { ElectionId = 1, TargetId = id }).ToList();

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenOrganization_ShouldReturnAllActiveEmployeesIgnoringTargets()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetActiveIdsAsync()).ReturnsAsync(new List<long> { 1, 2, 3 });

        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("Organization"), new List<ElectionAudienceTarget>());

        // Assert
        result.Should().BeEquivalentTo(new HashSet<long> { 1, 2, 3 });
    }

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenBranch_ShouldResolveBySiteId()
    {
        // Arrange
        _employeeRepositoryMock
            .Setup(r => r.GetActiveIdsBySiteIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(100))))
            .ReturnsAsync(new List<long> { 5, 6 });

        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("Branch"), Targets("100"));

        // Assert
        result.Should().BeEquivalentTo(new HashSet<long> { 5, 6 });
    }

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenDepartment_ShouldResolveByDepartmentId()
    {
        // Arrange
        _employeeRepositoryMock
            .Setup(r => r.GetActiveIdsByDepartmentIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(200))))
            .ReturnsAsync(new List<long> { 7 });

        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("Department"), Targets("200"));

        // Assert
        result.Should().BeEquivalentTo(new HashSet<long> { 7 });
    }

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenTeam_ShouldResolveByTeamMembership()
    {
        // Arrange
        _teamMemberRepositoryMock
            .Setup(r => r.GetActiveEmployeeIdsByTeamIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(300))))
            .ReturnsAsync(new List<long> { 8, 9 });

        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("Team"), Targets("300"));

        // Assert
        result.Should().BeEquivalentTo(new HashSet<long> { 8, 9 });
    }

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenSelectedEmployees_ShouldFilterToActiveOnly()
    {
        // Arrange
        _employeeRepositoryMock
            .Setup(r => r.FilterActiveIdsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(11) && ids.Contains(12))))
            .ReturnsAsync(new List<long> { 11 }); // 12 filtered out as inactive

        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("SelectedEmployees"), Targets("11", "12"));

        // Assert
        result.Should().BeEquivalentTo(new HashSet<long> { 11 });
    }

    [Fact]
    public async Task GetEligibleEmployeeIdsAsync_WhenNoTargetsConfigured_ShouldReturnEmptySet()
    {
        // Act
        var result = await _service.GetEligibleEmployeeIdsAsync(Election("Department"), new List<ElectionAudienceTarget>());

        // Assert
        result.Should().BeEmpty();
    }
}
