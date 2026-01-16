using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class TimelineTests
{
    [Fact]
    public void Timeline_Properties_CanBeSet()
    {
        // Arrange
        var timeline = new Timeline
        {
            StartTime = 2.5,
            Duration = 5.0
        };

        // Assert
        Assert.Equal(2.5, timeline.StartTime);
        Assert.Equal(5.0, timeline.Duration);
    }

    [Fact]
    public void Timeline_CanHaveZeroStartTime()
    {
        // Arrange
        var timeline = new Timeline
        {
            StartTime = 0.0,
            Duration = 3.0
        };

        // Assert
        Assert.Equal(0.0, timeline.StartTime);
    }

    [Fact]
    public void Timeline_CanHaveFractionalValues()
    {
        // Arrange
        var timeline = new Timeline
        {
            StartTime = 1.234,
            Duration = 0.567
        };

        // Assert
        Assert.Equal(1.234, timeline.StartTime);
        Assert.Equal(0.567, timeline.Duration);
    }
}
