using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.Integration;

[Trait("Category", "Integration")]
public class ViewModelIntegrationTests
{
    [Fact]
    public void CreateProjectAddSlidesSelectSlide_ViewModelState_CorrectlyMaintained()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Integration Test")
            .WithSlide(s => s.WithTitle("Slide 1"))
            .WithSlide(s => s.WithTitle("Slide 2"))
            .Build();

        // Act - Set project
        viewModel.CurrentProject = project;

        // Assert - First slide should be selected
        Assert.NotNull(viewModel.CurrentSlide);
        Assert.Equal("Slide 1", viewModel.CurrentSlide!.Title);

        // Act - Select second slide
        viewModel.CurrentSlide = project.Slides[1];

        // Assert
        Assert.Equal("Slide 2", viewModel.CurrentSlide.Title);
        Assert.Equal(2, viewModel.Slides.Count);
    }

    [Fact]
    public void CreateObjectSelectObjectEditProperties_ObjectModification_ReflectedInViewModel()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Object Test")
            .WithSlide(s => s.WithTitle("Test Slide"))
            .Build();

        viewModel.CurrentProject = project;
        Assert.NotNull(viewModel.CurrentSlide); // Ensure slide is set
        viewModel.SelectedTool = EditorTool.Text;

        // Act - Create object
        viewModel.CreateObjectAtPosition(100, 200);

        // Assert - Object created and selected
        Assert.NotNull(viewModel.SelectedObject);
        var textObject = viewModel.SelectedObject as TextObject;
        Assert.NotNull(textObject);
        Assert.Equal(100, textObject!.X);
        Assert.Equal(200, textObject.Y);

        // Act - Modify properties
        textObject.X = 300;
        textObject.Y = 400;
        textObject.Text = "Modified";

        // Assert - Changes reflected
        Assert.Equal(300, viewModel.SelectedObject.X);
        Assert.Equal(400, viewModel.SelectedObject.Y);
        Assert.Equal("Modified", textObject.Text);
    }

    [Fact]
    public void AddSlideDeleteSlide_CollectionOperations_ViewModelSynced()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Slide Operations")
            .Build();

        viewModel.CurrentProject = project;

        // Act - Add slides
        viewModel.AddSlideCommand.Execute(null);
        viewModel.AddSlideCommand.Execute(null);

        // Assert - ProjectBuilder creates 0 slides, so we expect 2 after adding 2
        Assert.Equal(2, project.Slides.Count);
        Assert.Equal(2, viewModel.Slides.Count);

        // Act - Delete a slide
        viewModel.CurrentSlide = project.Slides[0];
        viewModel.DeleteSlideCommand.Execute(null);

        // Assert
        Assert.Single(project.Slides);
        Assert.Single(viewModel.Slides);
    }

    [Fact]
    public void SelectToolCreateObject_ToolReset_AfterObjectCreation()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Tool Test")
            .WithSlide(s => s.WithTitle("Test Slide"))
            .Build();

        viewModel.CurrentProject = project;
        // Ensure CurrentSlide is set (should be set automatically, but verify)
        Assert.NotNull(viewModel.CurrentSlide);

        // Act - Select tool and create object
        viewModel.SelectedTool = EditorTool.Image;
        Assert.Equal(EditorTool.Image, viewModel.SelectedTool);

        viewModel.CreateObjectAtPosition(100, 100);

        // Assert - Tool should reset after creation
        Assert.Equal(EditorTool.None, viewModel.SelectedTool);
    }

    [Fact]
    public void DuplicateSlide_AllObjectsAndProperties_CopiedCorrectly()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Duplicate Test")
            .WithSlide(s => s
                .WithTitle("Original")
                .WithObject(o => o
                    .AtPosition(100, 200)
                    .WithSize(300, 50)
                    .BuildTextObject("Original Text", fontSize: 18, color: "#0000FF")))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];

        // Act - Duplicate slide
        viewModel.DuplicateSlideCommand.Execute(null);

        // Assert
        Assert.Equal(2, project.Slides.Count);
        var duplicated = project.Slides[1];
        Assert.Equal("Original (Copy)", duplicated.Title);
        Assert.Single(duplicated.Layers[0].Objects);
        
        var copiedObject = duplicated.Layers[0].Objects[0] as TextObject;
        Assert.NotNull(copiedObject);
        Assert.Equal(100, copiedObject!.X);
        Assert.Equal(200, copiedObject.Y);
        Assert.Equal(300, copiedObject.Width);
        Assert.Equal(50, copiedObject.Height);
        Assert.Equal("Original Text", copiedObject.Text);
        Assert.Equal(18, copiedObject.FontSize);
        Assert.Equal("#0000FF", copiedObject.Color);
        // ID should be different
        Assert.NotEqual(project.Slides[0].Layers[0].Objects[0].Id, copiedObject.Id);
    }

    [Fact]
    public void AddLayerToggleVisibility_LayerOperations_ViewModelUpdated()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Layer Test")
            .WithSlide(s => s.WithTitle("Test Slide"))
            .Build();

        viewModel.CurrentProject = project;
        Assert.NotNull(viewModel.CurrentSlide); // Ensure slide is set

        // Act - Add layer
        viewModel.AddLayerCommand.Execute(null);

        // Assert
        Assert.Equal(2, viewModel.CurrentSlide!.Layers.Count);
        Assert.Equal(2, viewModel.Layers.Count);

        // Act - Toggle visibility (modify the layer object, which is shared between collections)
        var newLayer = viewModel.CurrentSlide.Layers[1];
        newLayer.Visible = false;

        // Assert - Same object reference, so both should reflect the change
        Assert.False(newLayer.Visible);
        Assert.False(viewModel.Layers[1].Visible);
    }

    [Fact]
    public void WindowTitle_ProjectChanges_UpdatesCorrectly()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);

        // Assert - No project
        Assert.Equal("SlideForge", viewModel.WindowTitle);

        // Act - Set project
        var project = ProjectBuilder.Create()
            .WithName("Title Test")
            .Build();
        viewModel.CurrentProject = project;

        // Assert
        Assert.Equal("Title Test - SlideForge", viewModel.WindowTitle);
    }
}
