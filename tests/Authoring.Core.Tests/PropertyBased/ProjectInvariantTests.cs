using System.Linq;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Tests.Extensions;
using Authoring.Core.Tests.Helpers;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Authoring.Core.Tests.PropertyBased;

[Trait("Category", "PropertyBased")]
public class ProjectInvariantTests
{
    [Property]
    public bool Project_AllSlides_HaveAtLeastOneLayer(int slideCount)
    {
        slideCount = Math.Abs(slideCount % 10) + 1; // 1-10 slides

        // Arrange - Create project with multiple slides
        var builder = ProjectBuilder.Create().WithName("Test");
        for (int i = 0; i < slideCount; i++)
        {
            builder.WithSlide(s => s.WithTitle($"Slide {i}"));
        }
        var project = builder.Build();

        // Assert - Invariant: All slides must have at least one layer
        return project.Slides.All(s => s.Layers.Count > 0);
    }

    [Property]
    public bool Project_AllSlideIds_AreUnique(int slideCount)
    {
        slideCount = Math.Abs(slideCount % 20) + 1; // 1-20 slides

        // Arrange - Create project with multiple slides
        var builder = ProjectBuilder.Create().WithName("Test");
        for (int i = 0; i < slideCount; i++)
        {
            builder.WithSlide(s => s.WithTitle($"Slide {i}"));
        }
        var project = builder.Build();

        // Assert - Invariant: All slide IDs must be unique
        var slideIds = project.Slides.Select(s => s.Id).ToList();
        return slideIds.Count == slideIds.Distinct().Count();
    }

    [Property]
    public bool Project_AllVariableIds_AreUnique(int variableCount)
    {
        variableCount = Math.Abs(variableCount % 10); // 0-9 variables

        // Arrange - Create project with multiple variables
        var builder = ProjectBuilder.Create().WithName("Test");
        for (int i = 0; i < variableCount; i++)
        {
            builder.WithVariable($"var{i}", $"Variable {i}", VariableType.Number, 0);
        }
        var project = builder.Build();

        // Assert - Invariant: All variable IDs must be unique
        if (project.Variables.Count == 0)
            return true;

        var variableIds = project.Variables.Select(v => v.Id).ToList();
        return variableIds.Count == variableIds.Distinct().Count();
    }

    [Property]
    public bool Project_AllObjectIds_InLayerAreUnique(int objectCount)
    {
        objectCount = Math.Abs(objectCount % 15) + 1; // 1-15 objects

        // Arrange - Create project with multiple objects in the same layer
        var slideBuilder = new SlideBuilder().WithTitle("Test Slide");
        for (int i = 0; i < objectCount; i++)
        {
            slideBuilder.WithObject(o => o
                .AtPosition(i * 50, i * 50)
                .BuildTextObject($"Text {i}"));
        }
        
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(slideBuilder.Build())
            .Build();

        // Assert - Invariant: All object IDs within a layer must be unique
        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                var objectIds = layer.Objects.Select(o => o.Id).ToList();
                if (objectIds.Count != objectIds.Distinct().Count())
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Property]
    public bool Project_AllObjects_HaveValidDimensions(double width, double height)
    {
        // Filter valid dimensions
        if (width <= 0 || width > 10000 || height <= 0 || height > 10000)
            return true; // Skip invalid dimensions

        // Arrange - Create project with an object using the generated dimensions
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Test Slide")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .WithSize(width, height)
                    .BuildTextObject("Test")))
            .Build();

        // Assert - Invariant: All objects must have positive width and height
        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    if (obj.Width <= 0 || obj.Height <= 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    [Property]
    public bool Project_AllSlides_HavePositiveDimensions(double width, double height)
    {
        // Filter valid dimensions (including NaN and Infinity)
        if (double.IsNaN(width) || double.IsInfinity(width) || double.IsNaN(height) || double.IsInfinity(height) ||
            width <= 0 || width > 10000 || height <= 0 || height > 10000)
            return true; // Skip invalid dimensions

        // Arrange - Create project with a slide using the generated dimensions
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Test Slide")
                .WithDimensions(width, height))
            .Build();

        // Assert - Invariant: All slides must have positive width and height
        return project.Slides.All(s => s.Width > 0 && s.Height > 0);
    }

    [Property]
    public bool Project_AllObjects_HaveNonEmptyIds(int objectCount)
    {
        objectCount = Math.Abs(objectCount % 10) + 1; // 1-10 objects

        // Arrange - Create project with multiple objects
        var slideBuilder = new SlideBuilder().WithTitle("Test Slide");
        for (int i = 0; i < objectCount; i++)
        {
            slideBuilder.WithObject(o => o
                .AtPosition(i * 50, i * 50)
                .BuildTextObject($"Text {i}"));
        }
        
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(slideBuilder.Build())
            .Build();

        // Assert - Invariant: All objects must have non-empty IDs
        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    if (string.IsNullOrWhiteSpace(obj.Id))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    [Property]
    public bool Project_SerializationRoundTrip_MaintainsInvariants(int slideCount, int objectCount)
    {
        slideCount = Math.Abs(slideCount % 5) + 1; // 1-5 slides
        objectCount = Math.Abs(objectCount % 5); // 0-4 objects per slide

        // Arrange - Create a project with slides and objects
        var builder = ProjectBuilder.Create().WithName("Test");
        for (int i = 0; i < slideCount; i++)
        {
            var slideBuilder = new SlideBuilder().WithTitle($"Slide {i}");
            for (int j = 0; j < objectCount; j++)
            {
                slideBuilder.WithObject(o => o
                    .AtPosition(j * 50, j * 50)
                    .BuildTextObject($"Text {j}"));
            }
            builder.WithSlide(slideBuilder.Build());
        }
        var project = builder.Build();

        // Act - Serialize and deserialize
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert - All invariants should still hold
        if (deserialized == null)
            return false;

        var allSlidesHaveLayers = deserialized.Slides.All(s => s.Layers.Count > 0);
        var allSlideIdsUnique = deserialized.Slides.Select(s => s.Id).Distinct().Count() == deserialized.Slides.Count;
        var allVariableIdsUnique = deserialized.Variables.Count == 0 || 
            deserialized.Variables.Select(v => v.Id).Distinct().Count() == deserialized.Variables.Count;

        return allSlidesHaveLayers && allSlideIdsUnique && allVariableIdsUnique;
    }
}
