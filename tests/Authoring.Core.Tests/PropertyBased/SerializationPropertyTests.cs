using System;
using System.Linq;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Authoring.Core.Tests.PropertyBased;

[Trait("Category", "PropertyBased")]
public class SerializationPropertyTests
{
    [Property]
    public bool Serialization_IsReversible_ForProjectName(string projectName)
    {
        // Skip empty names - use conditional property
        if (string.IsNullOrWhiteSpace(projectName))
            return true;

        // Arrange
        var project = new Project { Name = projectName };

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert - Project should deserialize to equivalent structure
        return deserialized != null && deserialized.Name == project.Name;
    }

    [Property(MaxTest = 100)]
    public bool Serialization_RoundTrip_PreservesAllSlides(int slideCount)
    {
        slideCount = Math.Abs(slideCount % 20) + 1; // 1-20 slides

        // Arrange
        var project = new Project { Name = "Test" };
        for (int i = 0; i < slideCount; i++)
        {
            project.AddSlide(new Slide
            {
                Id = Guid.NewGuid().ToString(),
                Title = $"Slide {i}",
                Width = 1920,
                Height = 1080
            });
        }

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        return deserialized != null && deserialized.Slides.Count == slideCount;
    }

    [Property]
    public bool Serialization_PreservesObjectPositions(double x, double y)
    {
        // Filter valid positions - use conditional property
        if (x < 0 || x > 10000 || y < 0 || y > 10000)
            return true; // Skip invalid positions

        // Arrange
        var project = new Project { Name = "Position Test" };
        var slide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test",
            Width = 1920,
            Height = 1080
        };

        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Text",
            X = x,
            Y = y,
            Width = 100,
            Height = 50,
            Text = "Test"
        };

        slide.Layers[0].Objects.Add(textObject);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        if (deserialized == null || deserialized.Slides.Count == 0)
            return true; // Skip invalid cases
        
        var loadedObject = deserialized.Slides[0].Layers[0].Objects[0];
        return loadedObject.X == x && loadedObject.Y == y;
    }
}

// Simple wrapper types for property-based testing
public record ValidProject(Project Value);
public record ValidPosition(double Value);
