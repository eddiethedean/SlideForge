using System.Linq;
using System.Threading.Tasks;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Validation;
using Authoring.Desktop.Services;
using Xunit;

namespace Authoring.Desktop.Tests.Integration;

[Trait("Category", "Integration")]
public class TriggersVariablesAdvancedIntegrationTests
{
    [Fact]
    public void ComplexWorkflow_MultipleVariablesTriggersActions_SerializesCorrectly()
    {
        // Arrange - Create a complex scenario
        var project = ProjectBuilder.Create()
            .WithName("Complex Project")
            .WithVariable("score", "Score", VariableType.Number, 0)
            .WithVariable("level", "Level", VariableType.Number, 1)
            .WithVariable("completed", "Completed", VariableType.Boolean, false)
            .WithVariable("playerName", "Player Name", VariableType.String, "Player")
            .WithSlide(s => s
                .WithTitle("Start Screen")
                .WithId("start")
                .WithLayer("ui", "UI Layer", true)
                .WithLayer("game", "Game Layer", false)
                .WithObject(o => o.AtPosition(400, 300).BuildButtonObject("Start Button")))
            .WithSlide(s => s
                .WithTitle("Game Screen")
                .WithId("game")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Submit Button")))
            .WithSlide(s => s
                .WithTitle("End Screen")
                .WithId("end")
                .WithObject(o => o.AtPosition(200, 200).BuildTextObject("Score Display")))
            .Build();

        // Add complex triggers
        var startButton = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        startButton.Triggers.Add(new Trigger
        {
            Id = "startTrigger",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "score", Value = 0 },
                new SetVariableAction { VariableId = "level", Value = 1 },
                new SetVariableAction { VariableId = "completed", Value = false },
                new ShowLayerAction { LayerId = "game" },
                new NavigateToSlideAction { TargetSlideId = "game" }
            }
        });

        var submitButton = project.Slides[1].Layers[0].Objects.OfType<ButtonObject>().First();
        submitButton.Triggers.Add(new Trigger
        {
            Id = "submitTrigger",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "score", Value = 100 },
                new SetVariableAction { VariableId = "completed", Value = true },
                new NavigateToSlideAction { TargetSlideId = "end" }
            }
        });

        // Act - Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(4, deserialized!.Variables.Count);
        Assert.Equal(3, deserialized.Slides.Count);

        var deserializedStartButton = deserialized.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        Assert.Single(deserializedStartButton.Triggers);
        Assert.Equal(5, deserializedStartButton.Triggers[0].Actions.Count);

        var deserializedSubmitButton = deserialized.Slides[1].Layers[0].Objects.OfType<ButtonObject>().First();
        Assert.Single(deserializedSubmitButton.Triggers);
        Assert.Equal(3, deserializedSubmitButton.Triggers[0].Actions.Count);

        // Verify action types
        var actions = deserializedStartButton.Triggers[0].Actions;
        Assert.IsType<SetVariableAction>(actions[0]);
        Assert.IsType<SetVariableAction>(actions[1]);
        Assert.IsType<SetVariableAction>(actions[2]);
        Assert.IsType<ShowLayerAction>(actions[3]);
        Assert.IsType<NavigateToSlideAction>(actions[4]);
    }

    [Fact]
    public async Task SaveLoad_WithBrokenReferences_LoadsButValidatesWithWarnings()
    {
        // Arrange
        var service = new ProjectService();
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

        try
        {
            var project = ProjectBuilder.Create()
                .WithName("Broken References")
                .WithVariable("var1", "Var1", VariableType.Boolean, true)
                .WithSlide(s => s.WithTitle("Slide 1").WithId("slide1"))
                .WithSlide(s => s
                    .WithTitle("Slide 2")
                    .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
                .Build();

            var button = project.Slides[1].Layers[0].Objects.OfType<ButtonObject>().First();
            button.Triggers.Add(new Trigger
            {
                Id = "broken",
                Type = TriggerType.OnClick,
                Actions = new List<Action>
                {
                    new SetVariableAction { VariableId = "nonexistentVar", Value = true },
                    new NavigateToSlideAction { TargetSlideId = "nonexistentSlide" }
                }
            });

            // Act - Save and load (should succeed but have validation errors)
            await service.SaveProjectAsync(project, tempPath);
            var loaded = await service.OpenProjectAsync(tempPath);

            // Assert
            Assert.NotNull(loaded);
            var errors = ProjectValidator.ValidateProject(loaded!);
            Assert.Contains(errors, e => e.Contains("nonexistentVar"));
            Assert.Contains(errors, e => e.Contains("nonexistentSlide"));
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }

    [Fact]
    public void MultipleTriggers_SameObject_DifferentTypes_AllPreserved()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithLayer("layer1", "Layer 1", false)
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act - Add multiple triggers
        button.Triggers.Add(new Trigger
        {
            Id = "click1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new NavigateToSlideAction { TargetSlideId = project.Slides[0].Id } }
        });

        button.Triggers.Add(new Trigger
        {
            Id = "click2",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new ShowLayerAction { LayerId = "layer1" } }
        });

        button.Triggers.Add(new Trigger
        {
            Id = "timeline1",
            Type = TriggerType.OnTimelineStart,
            Actions = new List<Action> { new HideLayerAction { LayerId = "layer1" } }
        });

        // Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.NotNull(deserialized);
        var deserializedButton = deserialized!.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        Assert.Equal(3, deserializedButton.Triggers.Count);
        Assert.Equal(2, deserializedButton.Triggers.Count(t => t.Type == TriggerType.OnClick));
        Assert.Single(deserializedButton.Triggers.Where(t => t.Type == TriggerType.OnTimelineStart));
    }

    [Fact]
    public void Variable_ReferencedInMultipleTriggers_DeletionShowsMultipleWarnings()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("shared", "Shared", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button1"))
                .WithObject(o => o.AtPosition(200, 200).BuildButtonObject("Button2")))
            .Build();

        var button1 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var button2 = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().Skip(1).First();

        button1.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new SetVariableAction { VariableId = "shared", Value = 1 } }
        });

        button2.Triggers.Add(new Trigger
        {
            Id = "t2",
            Type = TriggerType.OnClick,
            Actions = new List<Action> { new SetVariableAction { VariableId = "shared", Value = 2 } }
        });

        var errors = ProjectValidator.ValidateProject(project);
        Assert.Empty(errors);

        // Act - Delete the shared variable
        var variable = project.Variables.First();
        project.Variables.Remove(variable);

        errors = ProjectValidator.ValidateProject(project);

        // Assert - Should have warnings for both triggers
        var sharedWarnings = errors.Where(e => e.Contains("shared")).ToList();
        Assert.True(sharedWarnings.Count >= 2, "Should have warnings for both triggers referencing deleted variable");
    }

    [Fact]
    public void LargeProject_ManyVariablesTriggersActions_SerializesAndValidates()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Large Project")
            .Build();

        // Add 50 variables
        for (int i = 0; i < 50; i++)
        {
            project.AddVariable(new Variable
            {
                Id = $"var{i}",
                Name = $"Variable{i}",
                Type = i % 3 == 0 ? VariableType.Boolean : (i % 3 == 1 ? VariableType.Number : VariableType.String),
                DefaultValue = i % 3 == 0 ? (i % 2 == 0) : (i % 3 == 1 ? (double)i : $"Value{i}")
            });
        }

        // Add 10 slides with objects and triggers
        for (int slideIndex = 0; slideIndex < 10; slideIndex++)
        {
            var slide = new Slide
            {
                Id = $"slide{slideIndex}",
                Title = $"Slide {slideIndex}",
                Width = 1920,
                Height = 1080
            };

            slide.Layers.Add(new Layer
            {
                Id = $"layer{slideIndex}",
                Name = "Base Layer",
                Visible = true
            });

            // Add 5 buttons per slide
            for (int btnIndex = 0; btnIndex < 5; btnIndex++)
            {
                var button = new ButtonObject
                {
                    Id = $"btn{slideIndex}_{btnIndex}",
                    Name = $"Button {btnIndex}",
                    X = btnIndex * 200,
                    Y = 100,
                    Width = 150,
                    Height = 40,
                    Visible = true
                };

                // Add trigger with multiple actions
                button.Triggers.Add(new Trigger
                {
                    Id = $"trigger{slideIndex}_{btnIndex}",
                    Type = TriggerType.OnClick,
                    Actions = new List<Action>
                    {
                        new SetVariableAction { VariableId = $"var{slideIndex * 5 + btnIndex}", Value = slideIndex * 5 + btnIndex },
                        new NavigateToSlideAction { TargetSlideId = $"slide{(slideIndex + 1) % 10}" }
                    }
                });

                slide.Layers[0].Objects.Add(button);
            }

            project.AddSlide(slide);
        }

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var errors = ProjectValidator.ValidateProject(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(deserialized);
        Assert.Equal(50, deserialized!.Variables.Count);
        Assert.Equal(10, deserialized.Slides.Count);
        Assert.All(deserialized.Slides, slide => Assert.Equal(5, slide.Layers[0].Objects.Count));
    }
}
