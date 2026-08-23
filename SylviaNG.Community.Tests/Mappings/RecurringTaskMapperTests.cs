using FluentAssertions;
using SylviaNG.Community.Application.Mappings;

namespace SylviaNG.Community.Tests.Mappings;

public class RecurringTaskMapperTests
{
    [Fact]
    public void GetNextOccurrence_WithDaily_ShouldAddIntervalDays()
    {
        // Arrange
        var from = new DateTime(2026, 1, 1);

        // Act
        var next = RecurringTaskMapper.GetNextOccurrence(from, "Daily", 3);

        // Assert
        next.Should().Be(new DateTime(2026, 1, 4));
    }

    [Fact]
    public void GetNextOccurrence_WithWeekly_ShouldAddIntervalWeeks()
    {
        // Arrange
        var from = new DateTime(2026, 1, 1);

        // Act
        var next = RecurringTaskMapper.GetNextOccurrence(from, "Weekly", 2);

        // Assert
        next.Should().Be(new DateTime(2026, 1, 15));
    }

    [Fact]
    public void GetNextOccurrence_WithMonthly_ShouldAddIntervalMonths()
    {
        // Arrange
        var from = new DateTime(2026, 1, 31);

        // Act
        var next = RecurringTaskMapper.GetNextOccurrence(from, "Monthly", 1);

        // Assert
        next.Should().Be(new DateTime(2026, 2, 28));
    }

    [Fact]
    public void GetNextOccurrence_IsCaseInsensitive()
    {
        // Arrange
        var from = new DateTime(2026, 1, 1);

        // Act
        var next = RecurringTaskMapper.GetNextOccurrence(from, "WEEKLY", 1);

        // Assert
        next.Should().Be(new DateTime(2026, 1, 8));
    }
}
