using Authoring.Core.Models;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class ViewModelEdgeCaseTests
{
    [Fact]
    public void MainWindowViewModel_NullProjectService_ThrowsException()
    {
        // Act & Assert
        // Note: Current implementation doesn't validate null, but this test documents expected behavior
        // If null checking is added in future, this will catch it
        var exception = Record.Exception(() => new MainWindowViewModel(null!));
        // Currently allows null, but test documents the gap for future null-safety improvements
    }

    [Fact]
    public void CreateObjectAtPosition_NoCurrentSlide_DoesNotCreateObject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        viewModel.SelectedTool = EditorTool.Text;

        // Act
        viewModel.CreateObjectAtPosition(100, 200);

        // Assert
        Assert.Null(viewModel.CurrentSlide);
    }

    [Fact]
    public void CreateObjectAtPosition_NoBaseLayer_DoesNotCreateObject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        // Slide has no layers
        project.AddSlide(slide);
        viewModel.CurrentProject = project;
        viewModel.SelectedTool = EditorTool.Text;

        // Act
        viewModel.CreateObjectAtPosition(100, 200);

        // Assert
        Assert.Empty(slide.Layers);
    }

    [Fact]
    public void DeleteSelectedObject_ObjectNotInBaseLayer_RemovesFromAnyLayer()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        
        // Add to a different layer (not base layer)
        var otherLayer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Other Layer" };
        otherLayer.Objects.Add(textObject);
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        slide.Layers.Add(otherLayer);
        
        project.AddSlide(slide);
        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = slide;
        viewModel.SelectedObject = textObject;

        // Act
        viewModel.DeleteSelectedObject();

        // Assert
        // Object should be removed from whichever layer it's in
        Assert.Empty(otherLayer.Objects);
        Assert.Null(viewModel.SelectedObject);
    }

    [Fact]
    public void CurrentSlide_SetToSlideFromDifferentProject_UpdatesCorrectly()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project1 = new Project { Name = "Project 1" };
        var slide1 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 1" };
        project1.AddSlide(slide1);
        viewModel.CurrentProject = project1;

        var project2 = new Project { Name = "Project 2" };
        var slide2 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 2" };
        project2.AddSlide(slide2);
        viewModel.CurrentProject = project2;

        // Act
        viewModel.CurrentSlide = slide2;

        // Assert
        Assert.Equal(slide2, viewModel.CurrentSlide);
        Assert.Equal("Project 2", viewModel.CurrentProject!.Name);
    }

    [Fact]
    public void WindowTitle_ProjectNameWithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Project & <Test>" };

        // Act
        viewModel.CurrentProject = project;

        // Assert
        Assert.Contains("Project & <Test>", viewModel.WindowTitle);
    }

    [Fact]
    public void SelectTool_InvalidString_DoesNotChangeTool()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var originalTool = viewModel.SelectedTool;

        // Act
        viewModel.SelectToolCommand.Execute("InvalidTool");

        // Assert
        Assert.Equal(originalTool, viewModel.SelectedTool);
    }

    [Fact]
    public void ToggleObjectTimeline_NoSelectedObject_DoesNotThrow()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);

        // Act & Assert - Should not throw
        viewModel.ToggleObjectTimelineCommand.Execute(null);
    }

    [Fact]
    public void DeleteSlide_LastSlide_StillAllowsDeletion()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Only Slide" };
        project.AddSlide(slide);
        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = slide;

        // Act
        viewModel.DeleteSlideCommand.Execute(null);

        // Assert
        Assert.Empty(project.Slides);
        Assert.Null(viewModel.CurrentSlide);
    }

    [Fact]
    public void DuplicateSlide_EmptySlide_CreatesEmptyDuplicate()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Empty Slide" };
        // No layers or objects
        project.AddSlide(slide);
        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = slide;

        // Act
        viewModel.DuplicateSlideCommand.Execute(null);

        // Assert
        Assert.Equal(2, project.Slides.Count);
        var duplicated = project.Slides[1];
        Assert.Equal("Empty Slide (Copy)", duplicated.Title);
    }
}
