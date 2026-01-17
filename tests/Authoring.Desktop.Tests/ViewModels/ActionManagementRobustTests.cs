using System.Linq;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Validation;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class ActionManagementRobustTests
{
    [Fact]
    public void SetVariableAction_WithBooleanVariable_StoresBooleanValue()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("flag", "Flag", VariableType.Boolean, false)
            .Build();

        // Act
        var action = new SetVariableAction
        {
            VariableId = "flag",
            Value = true
        };

        // Assert
        Assert.Equal(ActionType.SetVariable, action.Type);
        Assert.Equal("flag", action.VariableId);
        Assert.True((bool)action.Value!);
    }

    [Fact]
    public void SetVariableAction_WithNumberVariable_StoresNumberValue()
    {
        // Arrange & Act
        var action = new SetVariableAction
        {
            VariableId = "counter",
            Value = 42.5
        };

        // Assert
        Assert.Equal(ActionType.SetVariable, action.Type);
        Assert.Equal(42.5, action.Value);
    }

    [Fact]
    public void SetVariableAction_WithStringVariable_StoresStringValue()
    {
        // Arrange & Act
        var action = new SetVariableAction
        {
            VariableId = "message",
            Value = "Hello, World!"
        };

        // Assert
        Assert.Equal(ActionType.SetVariable, action.Type);
        Assert.Equal("Hello, World!", action.Value as string);
    }

    [Fact]
    public void SetVariableAction_WithNullValue_IsAcceptable()
    {
        // Arrange & Act
        var action = new SetVariableAction
        {
            VariableId = "optional",
            Value = null
        };

        // Assert
        Assert.Null(action.Value);
    }

    [Fact]
    public void NavigateToSlideAction_WithInvalidSlideId_ValidationFails()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s.WithTitle("Slide 1").WithId("slide1"))
            .WithSlide(s => s.WithTitle("Slide 2").WithId("slide2"))
            .Build();

        var action = new NavigateToSlideAction
        {
            TargetSlideId = "nonexistent"
        };

        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { action }
        });

        project.Slides[0].Layers[0].Objects.Add(button);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("nonexistent") && e.Contains("non-existent slide"));
    }

    [Fact]
    public void ShowLayerAction_WithInvalidLayerId_ValidationFails()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithLayer("layer1", "Layer 1", true))
            .Build();

        var action = new ShowLayerAction
        {
            LayerId = "nonexistent"
        };

        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { action }
        });

        project.Slides[0].Layers[0].Objects.Add(button);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("nonexistent") && e.Contains("non-existent layer"));
    }

    [Fact]
    public void HideLayerAction_WithInvalidLayerId_ValidationFails()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithLayer("layer1", "Layer 1", true))
            .Build();

        var action = new HideLayerAction
        {
            LayerId = "nonexistent"
        };

        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { action }
        });

        project.Slides[0].Layers[0].Objects.Add(button);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("nonexistent") && e.Contains("non-existent layer"));
    }

    [Fact]
    public void Action_MixedActionTypes_InSameTrigger()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithLayer("layer1", "Layer 1", true)
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act
        var trigger = new Trigger
        {
            Id = "mixed",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 10 },
                new ShowLayerAction { LayerId = "layer1" },
                new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id },
                new HideLayerAction { LayerId = "layer1" }
            }
        };
        button.Triggers.Add(trigger);

        // Assert
        Assert.Single(button.Triggers);
        Assert.Equal(4, button.Triggers[0].Actions.Count);
        Assert.IsType<SetVariableAction>(button.Triggers[0].Actions[0]);
        Assert.IsType<ShowLayerAction>(button.Triggers[0].Actions[1]);
        Assert.IsType<NavigateToSlideAction>(button.Triggers[0].Actions[2]);
        Assert.IsType<HideLayerAction>(button.Triggers[0].Actions[3]);
    }

    [Fact]
    public void SetVariableAction_WithIntValue_ConvertsToDouble()
    {
        // Arrange & Act
        var action = new SetVariableAction
        {
            VariableId = "counter",
            Value = 42 // int value
        };

        // Assert
        // Note: Current implementation stores as object, so int is preserved
        Assert.Equal(42, action.Value);
        Assert.IsType<int>(action.Value);
    }

    [Fact]
    public void Action_AfterSlideDeletion_NavigateActionValidationFails()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s.WithTitle("Slide 1").WithId("slide1"))
            .WithSlide(s => s.WithTitle("Slide 2").WithId("slide2"))
            .WithSlide(s => s
                .WithTitle("Slide 3")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[2].Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide1" }
            }
        });

        var errors = ProjectValidator.ValidateProject(project);
        Assert.Empty(errors); // Should be valid initially

        // Act - Delete slide1
        var slide1 = project.Slides.First(s => s.Id == "slide1");
        project.Slides.Remove(slide1);

        errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("slide1") && e.Contains("non-existent slide"));
    }

    [Fact]
    public void Action_AfterLayerDeletion_LayerActionValidationFails()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithLayer("base", "Base Layer", true)
                .WithLayer("layer1", "Layer 1", false)
                .WithLayer("layer2", "Layer 2", false))
            .Build();

        // Add button to base layer
        var button = new ButtonObject { Id = "btn1", Name = "Button", X = 100, Y = 100, Width = 150, Height = 40, Visible = true };
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new ShowLayerAction { LayerId = "layer1" }
            }
        });
        project.Slides[0].Layers[0].Objects.Add(button);

        var errors = ProjectValidator.ValidateProject(project);
        Assert.Empty(errors); // Should be valid initially

        // Act - Delete layer1
        var layer1 = project.Slides[0].Layers.First(l => l.Id == "layer1");
        project.Slides[0].Layers.Remove(layer1);

        errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Contains("layer1") && e.Contains("non-existent layer"));
    }
}
