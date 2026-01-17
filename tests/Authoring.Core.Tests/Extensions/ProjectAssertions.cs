using Authoring.Core.Models;
using Xunit;

namespace Authoring.Core.Tests.Extensions;

/// <summary>
/// Specialized assertion helpers for project-related tests.
/// </summary>
public static class ProjectAssertions
{
    /// <summary>
    /// Asserts that a project has the expected number of slides.
    /// </summary>
    public static void AssertSlideCount(Project project, int expectedCount)
    {
        Assert.Equal(expectedCount, project.Slides.Count);
    }

    /// <summary>
    /// Asserts that a project has the expected number of variables.
    /// </summary>
    public static void AssertVariableCount(Project project, int expectedCount)
    {
        Assert.Equal(expectedCount, project.Variables.Count);
    }

    /// <summary>
    /// Asserts that a project contains a slide with the specified title.
    /// </summary>
    public static Slide AssertProjectContainsSlide(Project project, string slideTitle)
    {
        var slide = project.Slides.FirstOrDefault(s => s.Title == slideTitle);
        Assert.NotNull(slide);
        return slide!;
    }

    /// <summary>
    /// Asserts that a project contains a variable with the specified name.
    /// </summary>
    public static Variable AssertProjectContainsVariable(Project project, string variableName)
    {
        var variable = project.Variables.FirstOrDefault(v => v.Name == variableName);
        Assert.NotNull(variable);
        return variable!;
    }

    /// <summary>
    /// Asserts that a slide has the expected number of layers.
    /// </summary>
    public static void AssertLayerCount(Slide slide, int expectedCount)
    {
        Assert.Equal(expectedCount, slide.Layers.Count);
    }

    /// <summary>
    /// Asserts that a slide has the expected number of objects in its base layer.
    /// </summary>
    public static void AssertObjectCount(Slide slide, int expectedCount)
    {
        var baseLayer = slide.Layers.FirstOrDefault();
        Assert.NotNull(baseLayer);
        Assert.Equal(expectedCount, baseLayer!.Objects.Count);
    }

    /// <summary>
    /// Asserts that a layer has the expected number of objects.
    /// </summary>
    public static void AssertObjectCount(Layer layer, int expectedCount)
    {
        Assert.Equal(expectedCount, layer.Objects.Count);
    }

    /// <summary>
    /// Asserts that a slide contains an object of the specified type and name.
    /// </summary>
    public static T AssertSlideContainsObject<T>(Slide slide, string objectName) where T : SlideObject
    {
        var obj = slide.Layers.SelectMany(l => l.Objects)
            .OfType<T>()
            .FirstOrDefault(o => o.Name == objectName);
        
        Assert.NotNull(obj);
        return obj!;
    }

    /// <summary>
    /// Asserts that an object has the expected position.
    /// </summary>
    public static void AssertObjectPosition(SlideObject obj, double expectedX, double expectedY)
    {
        Assert.Equal(expectedX, obj.X);
        Assert.Equal(expectedY, obj.Y);
    }

    /// <summary>
    /// Asserts that an object has the expected size.
    /// </summary>
    public static void AssertObjectSize(SlideObject obj, double expectedWidth, double expectedHeight)
    {
        Assert.Equal(expectedWidth, obj.Width);
        Assert.Equal(expectedHeight, obj.Height);
    }

    /// <summary>
    /// Asserts that serialization round-trip preserves project structure.
    /// </summary>
    public static void AssertSerializationRoundTrip(Project originalProject)
    {
        var json = Core.Serialization.ProjectJsonSerializer.Serialize(originalProject);
        var deserialized = Core.Serialization.ProjectJsonSerializer.Deserialize(json);
        
        AssertExtensions.AssertProjectsEqual(originalProject, deserialized, compareIds: true);
    }

    /// <summary>
    /// Asserts that a project maintains all invariants after an operation.
    /// </summary>
    public static void AssertProjectInvariants(Project project)
    {
        // All slides have at least one layer
        foreach (var slide in project.Slides)
        {
            Assert.NotEmpty(slide.Layers);
        }

        // All objects have valid IDs
        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    Assert.False(string.IsNullOrWhiteSpace(obj.Id));
                    Assert.False(string.IsNullOrWhiteSpace(obj.Name));
                }
            }
        }

        // All variables have valid IDs
        foreach (var variable in project.Variables)
        {
            Assert.False(string.IsNullOrWhiteSpace(variable.Id));
            Assert.False(string.IsNullOrWhiteSpace(variable.Name));
        }

        // All slides have unique IDs
        var slideIds = project.Slides.Select(s => s.Id).ToList();
        Assert.Equal(slideIds.Count, slideIds.Distinct().Count());

        // All variables have unique IDs
        var variableIds = project.Variables.Select(v => v.Id).ToList();
        Assert.Equal(variableIds.Count, variableIds.Distinct().Count());
    }
}
