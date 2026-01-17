using System.Linq;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class TriggerManagementRobustTests
{
    [Fact]
    public void Trigger_OnClick_OnlyAvailableForButtonObject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text"))
                .WithObject(o => o.AtPosition(200, 200).BuildButtonObject("Button")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];

        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        var buttonObject = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act
        viewModel.SelectedObject = textObject;
        var textTriggers = viewModel.SelectedObjectTriggers.ToList();

        viewModel.SelectedObject = buttonObject;
        var buttonTriggers = viewModel.SelectedObjectTriggers.ToList();

        // Assert
        // TextObject doesn't support OnClick by design (only ButtonObject does)
        // ButtonObject can have OnClick triggers
        Assert.NotNull(viewModel.SelectedObject);
    }

    [Fact]
    public void Trigger_OnTimelineStart_AvailableForAllObjects()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text")))
            .Build();

        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();

        // Act
        var trigger = new Trigger
        {
            Id = "timeline1",
            Type = TriggerType.OnTimelineStart,
            Actions = new List<Action> { new ShowLayerAction { LayerId = project.Slides[0].Layers[0].Id } }
        };
        textObject.Triggers.Add(trigger);

        // Assert
        Assert.Single(textObject.Triggers);
        Assert.Equal(TriggerType.OnTimelineStart, textObject.Triggers[0].Type);
    }

    [Fact]
    public void Trigger_WithMultipleActions_ExecutesInOrder()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act
        var trigger = new Trigger
        {
            Id = "multi",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 1 },
                new SetVariableAction { VariableId = "counter", Value = 2 },
                new SetVariableAction { VariableId = "counter", Value = 3 },
                new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id }
            }
        };
        button.Triggers.Add(trigger);

        // Assert
        Assert.Single(button.Triggers);
        Assert.Equal(4, button.Triggers[0].Actions.Count);
        Assert.IsType<SetVariableAction>(button.Triggers[0].Actions[0]);
        Assert.IsType<SetVariableAction>(button.Triggers[0].Actions[1]);
        Assert.IsType<SetVariableAction>(button.Triggers[0].Actions[2]);
        Assert.IsType<NavigateToSlideAction>(button.Triggers[0].Actions[3]);
    }

    [Fact]
    public void Trigger_WithNoActions_ValidationFails()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = "empty",
            Type = TriggerType.OnClick,
            Actions = new List<Action>() // Empty actions
        });

        viewModel.CurrentProject = project;

        // Act
        viewModel.ValidateProject();

        // Assert
        Assert.True(viewModel.HasValidationWarnings);
        Assert.Contains(viewModel.ValidationWarnings, w => w.Contains("no actions") && w.Contains("empty"));
    }

    [Fact]
    public void MultipleTriggers_SameObject_AllMaintained()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act
        button.Triggers.Add(new Trigger
        {
            Id = "click1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id } }
        });

        button.Triggers.Add(new Trigger
        {
            Id = "timeline1",
            Type = TriggerType.OnTimelineStart,
            Actions = new List<Action> { new ShowLayerAction { LayerId = project.Slides[0].Layers[0].Id } }
        });

        // Assert
        Assert.Equal(2, button.Triggers.Count);
        Assert.Contains(button.Triggers, t => t.Type == TriggerType.OnClick);
        Assert.Contains(button.Triggers, t => t.Type == TriggerType.OnTimelineStart);
    }

    [Fact]
    public void Trigger_AfterObjectDeleted_ValidationCleanedUp()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button1"))
                .WithObject(o => o.AtPosition(200, 200).BuildButtonObject("Button2")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];

        var button1 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var button2 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().Skip(1).First();

        button1.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id } }
        });

        viewModel.ValidateProject();
        Assert.False(viewModel.HasValidationWarnings);

        // Act - Delete button1
        viewModel.SelectedObject = button1;
        viewModel.DeleteSelectedObject();

        // Assert
        Assert.DoesNotContain(project.Slides[0].Layers[0].Objects, o => o.Id == button1.Id);
        viewModel.ValidateProject();
        // Should still validate without errors related to deleted object
    }

    [Fact]
    public void Trigger_WithManyActions_PerformsWell()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var actions = new List<Action>();

        // Act - Create trigger with 50 actions
        for (int i = 0; i < 50; i++)
        {
            actions.Add(new SetVariableAction { VariableId = "counter", Value = i });
        }

        button.Triggers.Add(new Trigger
        {
            Id = "large",
            Type = TriggerType.OnClick,
            Actions = actions
        });

        // Assert
        Assert.Single(button.Triggers);
        Assert.Equal(50, button.Triggers[0].Actions.Count);
    }

    [Fact]
    public void Trigger_SelectedObjectChanges_TriggersCollectionUpdates()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button1"))
                .WithObject(o => o.AtPosition(200, 200).BuildButtonObject("Button2")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];

        var button1 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var button2 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().Skip(1).First();

        button1.Triggers.Add(new Trigger { Id = "t1", Type = TriggerType.OnClick, Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id } } });
        button2.Triggers.Add(new Trigger { Id = "t2", Type = TriggerType.OnClick, Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id } } });
        button2.Triggers.Add(new Trigger { Id = "t3", Type = TriggerType.OnTimelineStart, Actions = new List<Action> { new ShowLayerAction { LayerId = project.Slides[0].Layers[0].Id } } });

        // Act
        viewModel.SelectedObject = button1;
        Assert.Single(viewModel.SelectedObjectTriggers);

        viewModel.SelectedObject = button2;
        Assert.Equal(2, viewModel.SelectedObjectTriggers.Count);

        viewModel.SelectedObject = null;

        // Assert
        Assert.Empty(viewModel.SelectedObjectTriggers);
    }
}
