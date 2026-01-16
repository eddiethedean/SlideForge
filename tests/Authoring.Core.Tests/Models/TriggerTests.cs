using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class TriggerTests
{
    [Fact]
    public void Trigger_InitializesWithEmptyActions()
    {
        // Act
        var trigger = new Trigger();

        // Assert
        Assert.NotNull(trigger.Actions);
        Assert.Empty(trigger.Actions);
    }

    [Fact]
    public void Trigger_Properties_CanBeSet()
    {
        // Arrange
        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            ObjectId = "btn1",
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" }
            }
        };

        // Assert
        Assert.Equal("trigger1", trigger.Id);
        Assert.Equal(TriggerType.OnClick, trigger.Type);
        Assert.Equal("btn1", trigger.ObjectId);
        Assert.Single(trigger.Actions);
    }

    [Fact]
    public void Trigger_ObjectId_CanBeNull_ForSlideLevelTriggers()
    {
        // Arrange
        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnTimelineStart,
            ObjectId = null
        };

        // Assert
        Assert.Null(trigger.ObjectId);
    }

    [Fact]
    public void Trigger_CanHaveMultipleActions()
    {
        // Arrange
        var trigger = new Trigger
        {
            Id = "trigger1",
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" },
                new SetVariableAction { VariableId = "var1", Value = true },
                new ShowLayerAction { LayerId = "layer1" }
            }
        };

        // Assert
        Assert.Equal(3, trigger.Actions.Count);
    }
}
