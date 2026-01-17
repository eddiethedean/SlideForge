using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Validation;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Tests.Extensions;

namespace Authoring.Core.Tests.Integration;

[Trait("Category", "Integration")]
public class FullWorkflowTests
{
    [Fact]
    public void CompleteWorkflow_CreateProject_AddSlides_AddObjects_AddTriggers_Save_Load_Verify()
    {
        // Arrange - Create project
        var project = ProjectBuilder.Create()
            .WithName("Workflow Test")
            .WithAuthor("Test User")
            .WithVariable("score", "Score", VariableType.Number, 0)
            .Build();

        // Act - Add slides
        var slide1 = SlideBuilder.Create()
            .WithTitle("Introduction")
            .WithObject(o => o
                .AtPosition(100, 100)
                .BuildTextObject("Welcome to the course"))
            .WithObject(o => o
                .AtPosition(800, 500)
                .BuildButtonObject("Next"))
            .Build();

        var slide2 = SlideBuilder.Create()
            .WithTitle("Content")
            .WithObject(o => o
                .AtPosition(200, 200)
                .BuildImageObject("content.png"))
            .Build();

        project.AddSlide(slide1);
        project.AddSlide(slide2);

        // Act - Add triggers
        var nextButton = slide1.Layers[0].Objects.OfType<ButtonObject>().First();
        nextButton.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = slide2.Id },
                new SetVariableAction { VariableId = "score", Value = 10 }
            }
        });

        // Act - Save and load
        var json = ProjectJsonSerializer.Serialize(project);
        var loadedProject = ProjectJsonSerializer.Deserialize(json);

        // Assert - Verify structure
        AssertExtensions.AssertProjectsEqual(project, loadedProject, compareIds: true);
        AssertProjectInvariants(loadedProject);
        AssertExtensions.AssertProjectIsValid(loadedProject);
    }

    [Fact]
    public void ComplexProjectWorkflow_AllObjectTypes_Variables_Triggers_Timelines()
    {
        // Arrange & Act - Create complex project
        var project = TestDataFactory.CreateComplexProject();

        // Act - Add timeline to objects
        var introSlide = project.Slides[0];
        var textObject = introSlide.Layers[0].Objects.OfType<TextObject>().First();
        textObject.Timeline = new Timeline { StartTime = 0, Duration = 3.0 };

        // Act - Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var loadedProject = ProjectJsonSerializer.Deserialize(json);

        // Assert - Verify all components
        Assert.Equal(project.Variables.Count, loadedProject.Variables.Count);
        Assert.Equal(project.Slides.Count, loadedProject.Slides.Count);
        
        var loadedTextObject = loadedProject.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        Assert.NotNull(loadedTextObject.Timeline);
        Assert.Equal(0, loadedTextObject.Timeline!.StartTime);
        Assert.Equal(3.0, loadedTextObject.Timeline.Duration);

        AssertExtensions.AssertProjectIsValid(loadedProject);
    }

    [Fact]
    public void MultiSlideNavigation_TriggersAndActions_CompleteFlow()
    {
        // Arrange - Create project with multiple slides
        var project = ProjectBuilder.Create()
            .WithName("Navigation Test")
            .Build();

        var slides = new List<Slide>();
        for (int i = 0; i < 5; i++)
        {
            var slide = SlideBuilder.Create()
                .WithTitle($"Slide {i + 1}")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject($"Go to Slide {(i + 1) % 5 + 1}"))
                .Build();
            project.AddSlide(slide);
            slides.Add(slide);
        }

        // Act - Add navigation triggers
        for (int i = 0; i < slides.Count; i++)
        {
            var button = slides[i].Layers[0].Objects.OfType<ButtonObject>().First();
            var targetSlideIndex = (i + 1) % slides.Count;
            button.Triggers.Add(new Trigger
            {
                Id = Guid.NewGuid().ToString(),
                Type = TriggerType.OnClick,
                Actions = new List<Authoring.Core.Models.Action>
                {
                    new NavigateToSlideAction { TargetSlideId = slides[targetSlideIndex].Id }
                }
            });
        }

        // Assert - Verify all triggers have valid references
        var errors = ProjectValidator.ValidateProject(project);
        Assert.Empty(errors);

        // Act - Serialize and verify round-trip
        ProjectAssertions.AssertSerializationRoundTrip(project);
    }

    [Fact]
    public void VariableManipulation_SetVariableActions_CompleteFlow()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Variable Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithVariable("done", "Done", VariableType.Boolean, false)
            .Build();

        var slide = SlideBuilder.Create()
            .WithTitle("Variables")
            .WithObject(o => o
                .AtPosition(100, 100)
                .BuildButtonObject("Increment"))
            .Build();

        project.AddSlide(slide);

        // Act - Add variable manipulation triggers
        var button = slide.Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 10 },
                new SetVariableAction { VariableId = "done", Value = true }
            }
        });

        // Act - Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var loadedProject = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var loadedButton = loadedProject.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var trigger = loadedButton.Triggers.First();
        Assert.Equal(2, trigger.Actions.Count);
        
        var setCounterAction = trigger.Actions.OfType<SetVariableAction>()
            .First(a => a.VariableId == "counter");
        Assert.Equal(10, setCounterAction.Value);

        AssertExtensions.AssertProjectIsValid(loadedProject);
    }

    [Fact]
    public void LayerVisibility_ShowHideLayerActions_CompleteFlow()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Layer Test")
            .Build();

        var slide = SlideBuilder.Create()
            .WithTitle("Layers")
            .WithLayer("base", "Base Layer")
            .WithLayer("overlay", "Overlay Layer", visible: false)
            .WithObject(o => o
                .AtPosition(100, 100)
                .BuildButtonObject("Toggle Overlay"))
            .Build();

        project.AddSlide(slide);

        // Act - Add layer visibility triggers
        var button = slide.Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new ShowLayerAction { LayerId = "overlay" }
            }
        });

        // Act - Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var loadedProject = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var loadedSlide = loadedProject.Slides[0];
        Assert.Equal(2, loadedSlide.Layers.Count);
        
        var overlayLayer = loadedSlide.Layers.First(l => l.Id == "overlay");
        Assert.False(overlayLayer.Visible); // Initially hidden

        var loadedButton = loadedSlide.Layers[0].Objects.OfType<ButtonObject>().First();
        var showAction = loadedButton.Triggers[0].Actions.OfType<ShowLayerAction>().First();
        Assert.Equal("overlay", showAction.LayerId);

        AssertExtensions.AssertProjectIsValid(loadedProject);
    }

    private void AssertProjectInvariants(Project project)
    {
        // All slides have at least one layer
        foreach (var slide in project.Slides)
        {
            Assert.NotEmpty(slide.Layers);
        }

        // All IDs are unique
        var allSlideIds = project.Slides.Select(s => s.Id).ToList();
        Assert.Equal(allSlideIds.Count, allSlideIds.Distinct().Count());

        var allVariableIds = project.Variables.Select(v => v.Id).ToList();
        Assert.Equal(allVariableIds.Count, allVariableIds.Distinct().Count());
    }
}
