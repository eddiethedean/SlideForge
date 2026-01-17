using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Validation;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Authoring.Core.Tests.PropertyBased;

[Trait("Category", "PropertyBased")]
public class ValidationPropertyTests
{
    [Property]
    public bool Validation_ValidProject_HasNoErrors(string projectName)
    {
        // Skip empty names
        if (string.IsNullOrWhiteSpace(projectName))
            return true;

        // Arrange - Create a valid project using ProjectBuilder
        var project = ProjectBuilder.Create()
            .WithName(projectName)
            .Build();

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert - Valid projects should have no errors
        return errors.Count == 0;
    }

    [Fact] // Convert to regular test since we're using fixed values
    public void Validation_ProjectWithEmptyName_HasErrors()
    {
        // Arrange & Act - Test with known empty strings
        var emptyNames = new[] { "", "   ", "\t", "\n" };
        foreach (var name in emptyNames)
        {
            var project = new Project { Name = name };
            var errors = ProjectValidator.ValidateProject(project);
            Assert.NotEmpty(errors);
        }
    }

    [Property]
    public bool Validation_ProjectWithUniqueSlideIds_IsValid(int slideCount)
    {
        slideCount = Math.Abs(slideCount % 50) + 1; // 1-50 slides

        // Arrange - Create project with unique slide IDs
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
        var errors = ProjectValidator.ValidateProject(project);

        // Assert - Should not have duplicate ID errors
        var hasDuplicates = errors.Any(e => e.Contains("duplicate") && e.Contains("id"));
        return !hasDuplicates;
    }

    [Fact] // Convert to regular test since we're using fixed values
    public void Validation_ProjectWithDuplicateSlideIds_HasErrors()
    {
        // Arrange - Create project with duplicate slide IDs
        var duplicateId = Guid.NewGuid().ToString();
        var project = new Project { Name = "Test" };
        
        project.AddSlide(new Slide { Id = duplicateId, Title = "Slide 1", Width = 1920, Height = 1080 });
        project.AddSlide(new Slide { Id = duplicateId, Title = "Slide 2", Width = 1920, Height = 1080 });

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert - Should have duplicate ID error
        var hasErrors = errors.Any(e => e.Contains("duplicate") || e.Contains("id"));
        Assert.True(hasErrors);
    }

    [Property]
    public bool Validation_ObjectsWithValidPositions_AreValid(double x, double y)
    {
        // Filter valid positions
        if (x < 0 || x > 10000 || y < 0 || y > 10000)
            return true; // Skip invalid positions

        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test",
            Width = 1920,
            Height = 1080
        };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer", Visible = true });

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
        var errors = ProjectValidator.ValidateProject(project);

        // Assert - Valid positions should not produce errors
        var hasPositionErrors = errors.Any(e => e.Contains("position") || e.Contains("X") || e.Contains("Y"));
        return !hasPositionErrors;
    }
}
