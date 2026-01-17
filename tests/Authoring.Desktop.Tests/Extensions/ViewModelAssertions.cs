using Authoring.Core.Models;
using Authoring.Desktop.ViewModels;
using Xunit;

namespace Authoring.Desktop.Tests.Extensions;

/// <summary>
/// Assertion helpers for ViewModel tests.
/// </summary>
public static class ViewModelAssertions
{
    /// <summary>
    /// Asserts that a ViewModel has no project loaded.
    /// </summary>
    public static void AssertNoProject(MainWindowViewModel viewModel)
    {
        Assert.Null(viewModel.CurrentProject);
        Assert.False(viewModel.HasProject);
        Assert.Empty(viewModel.Slides);
    }

    /// <summary>
    /// Asserts that a ViewModel has a project loaded.
    /// </summary>
    public static void AssertProjectLoaded(MainWindowViewModel viewModel, Project expectedProject)
    {
        Assert.NotNull(viewModel.CurrentProject);
        Assert.True(viewModel.HasProject);
        Assert.Equal(expectedProject.Name, viewModel.CurrentProject!.Name);
        Assert.Equal(expectedProject.Id, viewModel.CurrentProject.Id);
    }

    /// <summary>
    /// Asserts that the ViewModel's slides collection matches the project's slides.
    /// </summary>
    public static void AssertSlidesCollectionMatches(MainWindowViewModel viewModel, Project project)
    {
        Assert.Equal(project.Slides.Count, viewModel.Slides.Count);
        for (int i = 0; i < project.Slides.Count; i++)
        {
            Assert.Equal(project.Slides[i].Id, viewModel.Slides[i].Id);
            Assert.Equal(project.Slides[i].Title, viewModel.Slides[i].Title);
        }
    }

    /// <summary>
    /// Asserts that the ViewModel's layers collection matches the current slide's layers.
    /// </summary>
    public static void AssertLayersCollectionMatches(MainWindowViewModel viewModel, Slide slide)
    {
        Assert.Equal(slide.Layers.Count, viewModel.Layers.Count);
        for (int i = 0; i < slide.Layers.Count; i++)
        {
            Assert.Equal(slide.Layers[i].Id, viewModel.Layers[i].Id);
            Assert.Equal(slide.Layers[i].Name, viewModel.Layers[i].Name);
        }
    }

    /// <summary>
    /// Asserts that a ViewModel has a slide selected.
    /// </summary>
    public static void AssertSlideSelected(MainWindowViewModel viewModel, Slide? expectedSlide)
    {
        Assert.Equal(expectedSlide, viewModel.CurrentSlide);
        if (expectedSlide != null)
        {
            Assert.Equal(expectedSlide.Id, viewModel.CurrentSlide!.Id);
            Assert.Equal(expectedSlide.Title, viewModel.CurrentSlide.Title);
        }
    }

    /// <summary>
    /// Asserts that a ViewModel has an object selected.
    /// </summary>
    public static void AssertObjectSelected(MainWindowViewModel viewModel, SlideObject? expectedObject)
    {
        Assert.Equal(expectedObject, viewModel.SelectedObject);
        if (expectedObject != null)
        {
            Assert.Equal(expectedObject.Id, viewModel.SelectedObject!.Id);
        }
    }

    /// <summary>
    /// Asserts that the window title reflects the project name.
    /// </summary>
    public static void AssertWindowTitle(MainWindowViewModel viewModel, string expectedProjectName, bool isModified = false)
    {
        var expectedTitle = isModified 
            ? $"{expectedProjectName} * - SlideForge"
            : $"{expectedProjectName} - SlideForge";
        Assert.Equal(expectedTitle, viewModel.WindowTitle);
    }

    /// <summary>
    /// Asserts that a tool is selected.
    /// </summary>
    public static void AssertToolSelected(MainWindowViewModel viewModel, Models.EditorTool expectedTool)
    {
        Assert.Equal(expectedTool, viewModel.SelectedTool);
    }

    /// <summary>
    /// Asserts that typed object properties return the correct objects.
    /// </summary>
    public static void AssertTypedObjectProperties(MainWindowViewModel viewModel, SlideObject? obj)
    {
        if (obj == null)
        {
            Assert.Null(viewModel.SelectedTextObject);
            Assert.Null(viewModel.SelectedImageObject);
            Assert.Null(viewModel.SelectedButtonObject);
            return;
        }

        switch (obj)
        {
            case TextObject textObj:
                Assert.Equal(textObj, viewModel.SelectedTextObject);
                Assert.Null(viewModel.SelectedImageObject);
                Assert.Null(viewModel.SelectedButtonObject);
                break;

            case ImageObject imageObj:
                Assert.Null(viewModel.SelectedTextObject);
                Assert.Equal(imageObj, viewModel.SelectedImageObject);
                Assert.Null(viewModel.SelectedButtonObject);
                break;

            case ButtonObject buttonObj:
                Assert.Null(viewModel.SelectedTextObject);
                Assert.Null(viewModel.SelectedImageObject);
                Assert.Equal(buttonObj, viewModel.SelectedButtonObject);
                break;
        }
    }
}
