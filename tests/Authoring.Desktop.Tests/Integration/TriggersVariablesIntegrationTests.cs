using System.Linq;
using System.Threading.Tasks;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.Integration;

[Trait("Category", "Integration")]
public class TriggersVariablesIntegrationTests
{
    [Fact]
    public void CreateVariableAddTriggerWithAction_SerializationRoundTrip_PreservesData()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Triggers Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Click Me")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 10 },
                new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id }
            }
        };
        button.Triggers.Add(trigger);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Single(deserialized.Variables);
        Assert.Equal("counter", deserialized.Variables[0].Id);

        var deserializedButton = deserialized.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        Assert.Single(deserializedButton.Triggers);
        Assert.Equal(TriggerType.OnClick, deserializedButton.Triggers[0].Type);
        Assert.Equal(2, deserializedButton.Triggers[0].Actions.Count);

        var setVarAction = deserializedButton.Triggers[0].Actions[0] as SetVariableAction;
        Assert.NotNull(setVarAction);
        Assert.Equal("counter", setVarAction!.VariableId);
        Assert.Equal(10, setVarAction.Value);

        var navAction = deserializedButton.Triggers[0].Actions[1] as NavigateToSlideAction;
        Assert.NotNull(navAction);
        Assert.Equal(project.Slides[0].Id, navAction!.TargetSlideId);
    }

    [Fact]
    public void CreateVariableDeleteVariable_ValidationShowsWarning()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("var1", "Variable1", VariableType.Boolean, true)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "var1", Value = false }
            }
        });

        viewModel.CurrentProject = project;
        viewModel.ValidateProject(); // Initial validation should pass
        Assert.False(viewModel.HasValidationWarnings); // Should have no warnings initially

        // Act
        var variable = project.Variables.First();
        project.Variables.Remove(variable);
        viewModel.UpdateVariablesCollection();
        viewModel.ValidateProject(); // Re-validate after deletion

        // Assert
        Assert.True(viewModel.HasValidationWarnings);
        Assert.NotEmpty(viewModel.ValidationWarnings);
        Assert.Contains("var1", viewModel.ValidationWarnings.First());
    }

    [Fact]
    public void CreateTriggerWithNavigateActionDeleteSlide_ValidationShowsWarning()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s.WithTitle("Slide 1").WithId("slide1"))
            .WithSlide(s => s.WithTitle("Slide 2").WithId("slide2"))
            .WithSlide(s => s
                .WithTitle("Slide 3")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button")))
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

        viewModel.CurrentProject = project;
        viewModel.ValidateProject(); // Initial validation should pass
        Assert.False(viewModel.HasValidationWarnings); // Should have no warnings initially

        // Act
        var slide1 = project.Slides.First(s => s.Id == "slide1");
        project.Slides.Remove(slide1);
        viewModel.UpdateSlidesCollection();
        viewModel.ValidateProject(); // Re-validate after deletion

        // Assert
        Assert.True(viewModel.HasValidationWarnings);
        Assert.Contains("slide1", viewModel.ValidationWarnings.First());
    }

    [Fact]
    public async Task CreateVariableTriggerAction_SaveAndLoad_PreservesEverything()
    {
        // Arrange
        var service = new ProjectService();
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

        try
        {
            var project = ProjectBuilder.Create()
                .WithName("Full Test")
                .WithVariable("score", "Score", VariableType.Number, 0)
                .WithSlide(s => s
                    .WithTitle("Main")
                    .WithObject(o => o
                        .AtPosition(100, 100)
                        .BuildButtonObject("Submit")))
                .Build();

            var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
            button.Triggers.Add(new Trigger
            {
                Id = "submit",
                Type = TriggerType.OnClick,
                Actions = new List<Action>
                {
                    new SetVariableAction { VariableId = "score", Value = 100 },
                    new ShowLayerAction { LayerId = project.Slides[0].Layers[0].Id }
                }
            });

            // Act
            await service.SaveProjectAsync(project, tempPath);
            var loaded = await service.OpenProjectAsync(tempPath);

            // Assert
            Assert.NotNull(loaded);
            Assert.Single(loaded!.Variables);
            Assert.Equal("score", loaded.Variables[0].Id);

            var loadedButton = loaded.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
            Assert.Single(loadedButton.Triggers);
            Assert.Equal(2, loadedButton.Triggers[0].Actions.Count);

            var setVar = loadedButton.Triggers[0].Actions[0] as SetVariableAction;
            Assert.NotNull(setVar);
            Assert.Equal("score", setVar!.VariableId);
            Assert.Equal(100, setVar.Value);

            var showLayer = loadedButton.Triggers[0].Actions[1] as ShowLayerAction;
            Assert.NotNull(showLayer);
            Assert.Equal(project.Slides[0].Layers[0].Id, showLayer!.LayerId);
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
