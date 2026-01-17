using System.Linq;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class TriggerManagementTests
{
    [Fact]
    public void AddTrigger_ToObject_AddsTriggerToObject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];
        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        viewModel.SelectedObject = button;

        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = "slide1" } }
        };

        // Act
        button.Triggers.Add(trigger);
        viewModel.UpdateSelectedObjectTriggers();

        // Assert
        Assert.Single(button.Triggers);
        Assert.Single(viewModel.SelectedObjectTriggers);
        Assert.Equal(trigger, viewModel.SelectedObjectTriggers[0]);
    }

    [Fact]
    public void DeleteTrigger_FromObject_RemovesTrigger()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];
        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        viewModel.SelectedObject = button;

        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>()
        };
        button.Triggers.Add(trigger);
        viewModel.UpdateSelectedObjectTriggers();

        // Act
        viewModel.DeleteTrigger(trigger);

        // Assert
        Assert.Empty(button.Triggers);
        Assert.Empty(viewModel.SelectedObjectTriggers);
    }

    [Fact]
    public void SelectedObjectTriggers_WhenObjectChanges_UpdatesCollection()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button1"))
                .WithObject(o => o
                    .AtPosition(200, 200)
                    .BuildButtonObject("Button2")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];

        var button1 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var button2 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().Skip(1).First();

        button1.Triggers.Add(new Trigger { Id = "t1", Type = TriggerType.OnClick, Actions = new List<Action>() });
        button2.Triggers.Add(new Trigger { Id = "t2", Type = TriggerType.OnClick, Actions = new List<Action>() });

        // Act
        viewModel.SelectedObject = button1;
        Assert.Single(viewModel.SelectedObjectTriggers);
        Assert.Equal("t1", viewModel.SelectedObjectTriggers[0].Id);

        viewModel.SelectedObject = button2;

        // Assert
        Assert.Single(viewModel.SelectedObjectTriggers);
        Assert.Equal("t2", viewModel.SelectedObjectTriggers[0].Id);
    }

    [Fact]
    public void Trigger_WithNavigateToSlideAction_HasCorrectTarget()
    {
        // Arrange & Act
        var action = new NavigateToSlideAction
        {
            TargetSlideId = "slide2"
        };

        // Assert
        Assert.Equal(ActionType.NavigateToSlide, action.Type);
        Assert.Equal("slide2", action.TargetSlideId);
    }

    [Fact]
    public void Trigger_WithSetVariableAction_HasCorrectVariableAndValue()
    {
        // Arrange & Act
        var action = new SetVariableAction
        {
            VariableId = "counter",
            Value = 42
        };

        // Assert
        Assert.Equal(ActionType.SetVariable, action.Type);
        Assert.Equal("counter", action.VariableId);
        Assert.Equal(42, action.Value);
    }

    [Fact]
    public void Trigger_WithShowLayerAction_HasCorrectLayerId()
    {
        // Arrange & Act
        var action = new ShowLayerAction
        {
            LayerId = "layer1"
        };

        // Assert
        Assert.Equal(ActionType.ShowLayer, action.Type);
        Assert.Equal("layer1", action.LayerId);
    }

    [Fact]
    public void Trigger_WithHideLayerAction_HasCorrectLayerId()
    {
        // Arrange & Act
        var action = new HideLayerAction
        {
            LayerId = "layer1"
        };

        // Assert
        Assert.Equal(ActionType.HideLayer, action.Type);
        Assert.Equal("layer1", action.LayerId);
    }
}
