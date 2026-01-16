using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class ActionTests
{
    [Fact]
    public void NavigateToSlideAction_HasCorrectType()
    {
        // Act
        var action = new NavigateToSlideAction();

        // Assert
        Assert.Equal(ActionType.NavigateToSlide, action.Type);
    }

    [Fact]
    public void NavigateToSlideAction_Properties_CanBeSet()
    {
        // Arrange
        var action = new NavigateToSlideAction { TargetSlideId = "slide2" };

        // Assert
        Assert.Equal("slide2", action.TargetSlideId);
    }

    [Fact]
    public void SetVariableAction_HasCorrectType()
    {
        // Act
        var action = new SetVariableAction();

        // Assert
        Assert.Equal(ActionType.SetVariable, action.Type);
    }

    [Fact]
    public void SetVariableAction_Properties_CanBeSet()
    {
        // Arrange
        var action = new SetVariableAction
        {
            VariableId = "var1",
            Value = true
        };

        // Assert
        Assert.Equal("var1", action.VariableId);
        Assert.Equal(true, action.Value);
    }

    [Fact]
    public void SetVariableAction_CanSetDifferentValueTypes()
    {
        // Arrange & Act
        var actionBool = new SetVariableAction { VariableId = "var1", Value = true };
        var actionNumber = new SetVariableAction { VariableId = "var2", Value = 42 };
        var actionString = new SetVariableAction { VariableId = "var3", Value = "test" };

        // Assert
        Assert.True((bool)actionBool.Value!);
        Assert.Equal(42, actionNumber.Value);
        Assert.Equal("test", actionString.Value);
    }

    [Fact]
    public void ShowLayerAction_HasCorrectType()
    {
        // Act
        var action = new ShowLayerAction();

        // Assert
        Assert.Equal(ActionType.ShowLayer, action.Type);
    }

    [Fact]
    public void ShowLayerAction_Properties_CanBeSet()
    {
        // Arrange
        var action = new ShowLayerAction { LayerId = "layer1" };

        // Assert
        Assert.Equal("layer1", action.LayerId);
    }

    [Fact]
    public void HideLayerAction_HasCorrectType()
    {
        // Act
        var action = new HideLayerAction();

        // Assert
        Assert.Equal(ActionType.HideLayer, action.Type);
    }

    [Fact]
    public void HideLayerAction_Properties_CanBeSet()
    {
        // Arrange
        var action = new HideLayerAction { LayerId = "layer1" };

        // Assert
        Assert.Equal("layer1", action.LayerId);
    }
}
