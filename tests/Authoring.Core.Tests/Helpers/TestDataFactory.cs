using Authoring.Core.Models;

namespace Authoring.Core.Tests.Helpers;

/// <summary>
/// Factory methods for creating common test scenarios.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a minimal valid project with one slide and base layer.
    /// </summary>
    public static Project CreateMinimalProject()
    {
        return ProjectBuilder.Create()
            .WithName("Minimal Project")
            .Build();
    }

    /// <summary>
    /// Creates a project with multiple slides, each with objects.
    /// </summary>
    public static Project CreateMultiSlideProject(int slideCount = 3)
    {
        var builder = ProjectBuilder.Create()
            .WithName("Multi-Slide Project")
            .WithAuthor("Test Author");

        for (int i = 1; i <= slideCount; i++)
        {
            builder.WithSlide(s => s
                .WithTitle($"Slide {i}")
                .WithObject(o => o
                    .AtPosition(i * 100, i * 100)
                    .BuildTextObject($"Text on Slide {i}")));
        }

        return builder.Build();
    }

    /// <summary>
    /// Creates a complex project with all object types, variables, and triggers.
    /// </summary>
    public static Project CreateComplexProject()
    {
        var project = ProjectBuilder.Create()
            .WithName("Complex Project")
            .WithAuthor("Test Author")
            .WithVariable("score", "Score", VariableType.Number, 0)
            .WithVariable("completed", "Completed", VariableType.Boolean, false)
            .WithSlide(s => s
                .WithTitle("Introduction")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildTextObject("Welcome"))
                .WithObject(o => o
                    .AtPosition(100, 200)
                    .BuildButtonObject("Next")))
            .WithSlide(s => s
                .WithTitle("Content")
                .WithObject(o => o
                    .AtPosition(200, 200)
                    .BuildImageObject("image.png")))
            .Build();

        // Add triggers to the button
        var button = project.Slides[0].Layers[0].Objects
            .OfType<ButtonObject>()
            .First();
        
        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction
                {
                    TargetSlideId = project.Slides[1].Id
                }
            }
        });

        return project;
    }

    /// <summary>
    /// Creates a project with all variable types.
    /// </summary>
    public static Project CreateProjectWithAllVariableTypes()
    {
        return ProjectBuilder.Create()
            .WithName("Variable Types Project")
            .WithVariable("boolVar", "Boolean Variable", VariableType.Boolean, true)
            .WithVariable("numVar", "Number Variable", VariableType.Number, 42)
            .WithVariable("strVar", "String Variable", VariableType.String, "Hello")
            .Build();
    }

    /// <summary>
    /// Creates a project with objects having timelines.
    /// </summary>
    public static Project CreateProjectWithTimelines()
    {
        return ProjectBuilder.Create()
            .WithName("Timeline Project")
            .WithSlide(s => s
                .WithTitle("Animated Slide")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .WithTimeline(0, 5.0)
                    .BuildTextObject("Animated Text"))
                .WithObject(o => o
                    .AtPosition(200, 200)
                    .WithTimeline(2.0, 3.0)
                    .BuildImageObject("image.png")))
            .Build();
    }

    /// <summary>
    /// Creates a project with multiple layers per slide.
    /// </summary>
    public static Project CreateProjectWithLayers()
    {
        return ProjectBuilder.Create()
            .WithName("Layered Project")
            .WithSlide(s => s
                .WithTitle("Multi-Layer Slide")
                .WithLayer("layer1", "Background Layer")
                .WithLayer("layer2", "Foreground Layer")
                .WithObject(o => o
                    .AtPosition(0, 0)
                    .WithName("Background Object")
                    .BuildTextObject("Background"))
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .WithName("Foreground Object")
                    .BuildTextObject("Foreground")))
            .Build();
    }

    /// <summary>
    /// Creates a large project with many slides and objects (for performance testing).
    /// </summary>
    public static Project CreateLargeProject(int slides = 50, int objectsPerSlide = 20)
    {
        var builder = ProjectBuilder.Create()
            .WithName($"Large Project ({slides} slides)");

        for (int i = 1; i <= slides; i++)
        {
            var slideBuilder = new SlideBuilder()
                .WithTitle($"Slide {i}");

            for (int j = 1; j <= objectsPerSlide; j++)
            {
                slideBuilder.WithObject(o => o
                    .AtPosition(j * 50, j * 30)
                    .WithName($"Object {j}")
                    .BuildTextObject($"Text {j}"));
            }

            builder.WithSlide(slideBuilder.Build());
        }

        return builder.Build();
    }

    /// <summary>
    /// Creates a project with invalid references (for validation testing).
    /// </summary>
    public static Project CreateInvalidProject()
    {
        var project = ProjectBuilder.Create()
            .WithName("Invalid Project")
            .WithSlide(s => s.WithId("slide1").WithTitle("Slide 1"))
            .Build();

        // Add a trigger with invalid slide reference
        var slide = project.Slides[0];
        var button = new ButtonObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Invalid Button",
            X = 100,
            Y = 100,
            Width = 150,
            Height = 40,
            Label = "Invalid"
        };

        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction
                {
                    TargetSlideId = "nonexistent-slide-id" // Invalid reference
                }
            }
        });

        slide.Layers[0].Objects.Add(button);

        return project;
    }
}
