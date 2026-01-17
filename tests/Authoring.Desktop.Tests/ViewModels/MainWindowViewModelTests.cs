using Authoring.Core.Models;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class MainWindowViewModelTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly MainWindowViewModel _viewModel;

    public MainWindowViewModelTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _viewModel = new MainWindowViewModel(_mockProjectService.Object);
    }

    [Fact]
    public void Constructor_InitializesEmptyCollections()
    {
        // Assert
        Assert.NotNull(_viewModel.Slides);
        Assert.NotNull(_viewModel.Layers);
        Assert.Empty(_viewModel.Slides);
        Assert.Empty(_viewModel.Layers);
        Assert.Null(_viewModel.CurrentProject);
        Assert.Null(_viewModel.CurrentSlide);
        Assert.Null(_viewModel.SelectedObject);
        Assert.Equal(EditorTool.None, _viewModel.SelectedTool);
    }

    [Fact]
    public void WindowTitle_NoProject_ReturnsDefaultTitle()
    {
        // Assert
        Assert.Equal("SlideForge", _viewModel.WindowTitle);
    }

    [Fact]
    public void WindowTitle_WithProject_ReturnsProjectName()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        _viewModel.CurrentProject = project;

        // Assert
        Assert.Equal("Test Project - SlideForge", _viewModel.WindowTitle);
    }

    [Fact]
    public void WindowTitle_WithProject_UpdatesOnProjectChange()
    {
        // Arrange
        var project1 = new Project { Name = "Project 1" };
        var project2 = new Project { Name = "Project 2" };

        // Act
        _viewModel.CurrentProject = project1;
        var title1 = _viewModel.WindowTitle;
        _viewModel.CurrentProject = project2;
        var title2 = _viewModel.WindowTitle;

        // Assert
        Assert.Contains("Project 1", title1);
        Assert.Contains("Project 2", title2);
    }

    [Fact]
    public void HasProject_NoProject_ReturnsFalse()
    {
        // Assert
        Assert.False(_viewModel.HasProject);
    }

    [Fact]
    public void HasProject_WithProject_ReturnsTrue()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        _viewModel.CurrentProject = project;

        // Assert
        Assert.True(_viewModel.HasProject);
    }

    [Fact]
    public void CurrentProject_SettingProject_UpdatesSlidesCollection()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var slide1 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 1" };
        var slide2 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 2" };
        project.AddSlide(slide1);
        project.AddSlide(slide2);

        // Act
        _viewModel.CurrentProject = project;

        // Assert
        Assert.Equal(2, _viewModel.Slides.Count);
        Assert.Equal("Slide 1", _viewModel.Slides[0].Title);
        Assert.Equal("Slide 2", _viewModel.Slides[1].Title);
    }

    [Fact]
    public void CurrentProject_SettingProject_SelectsFirstSlide()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "First Slide" };
        project.AddSlide(slide);

        // Act
        _viewModel.CurrentProject = project;

        // Assert
        Assert.NotNull(_viewModel.CurrentSlide);
        Assert.Equal("First Slide", _viewModel.CurrentSlide!.Title);
    }

    [Fact]
    public void CurrentSlide_SettingSlide_UpdatesLayersCollection()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Test Slide" };
        var layer1 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Layer 1" };
        var layer2 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Layer 2" };
        slide.Layers.Add(layer1);
        slide.Layers.Add(layer2);

        // Act
        _viewModel.CurrentSlide = slide;

        // Assert
        Assert.Equal(2, _viewModel.Layers.Count);
        Assert.Equal("Layer 1", _viewModel.Layers[0].Name);
        Assert.Equal("Layer 2", _viewModel.Layers[1].Name);
    }

    [Fact]
    public void CurrentSlide_SettingSlide_ClearsSelectedObject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide1 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 1" };
        var layer1 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" };
        slide1.Layers.Add(layer1);
        project.AddSlide(slide1);
        _viewModel.CurrentProject = project;

        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        _viewModel.CurrentSlide!.Layers[0].Objects.Add(textObject);
        _viewModel.SelectedObject = textObject;

        var slide2 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 2" };
        var layer2 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" };
        slide2.Layers.Add(layer2);

        // Act
        _viewModel.CurrentSlide = slide2;

        // Assert
        Assert.Null(_viewModel.SelectedObject);
    }

    [Fact]
    public void SelectTool_WithEnum_UpdatesSelectedTool()
    {
        // Act
        _viewModel.SelectToolCommand.Execute(EditorTool.Text);

        // Assert
        Assert.Equal(EditorTool.Text, _viewModel.SelectedTool);
    }

    [Fact]
    public void SelectTool_WithString_UpdatesSelectedTool()
    {
        // Act
        _viewModel.SelectToolCommand.Execute("Image");

        // Assert
        Assert.Equal(EditorTool.Image, _viewModel.SelectedTool);
    }

    [Fact]
    public void CreateObjectAtPosition_WithTextTool_CreatesTextObject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        _viewModel.SelectedTool = EditorTool.Text;

        // Act
        _viewModel.CreateObjectAtPosition(100, 200);

        // Assert
        Assert.Single(_viewModel.CurrentSlide!.Layers[0].Objects);
        var textObject = _viewModel.CurrentSlide.Layers[0].Objects[0] as TextObject;
        Assert.NotNull(textObject);
        Assert.Equal(100, textObject!.X);
        Assert.Equal(200, textObject.Y);
        Assert.Equal(200, textObject.Width);
        Assert.Equal(50, textObject.Height);
        Assert.Equal("Text", textObject.Text);
        Assert.Equal(_viewModel.SelectedObject, textObject);
    }

    [Fact]
    public void CreateObjectAtPosition_WithImageTool_CreatesImageObject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        _viewModel.SelectedTool = EditorTool.Image;

        // Act
        _viewModel.CreateObjectAtPosition(150, 250);

        // Assert
        Assert.Single(_viewModel.CurrentSlide!.Layers[0].Objects);
        var imageObject = _viewModel.CurrentSlide.Layers[0].Objects[0] as ImageObject;
        Assert.NotNull(imageObject);
        Assert.Equal(150, imageObject!.X);
        Assert.Equal(250, imageObject.Y);
        Assert.Equal(100, imageObject.Width);
        Assert.Equal(100, imageObject.Height);
        Assert.Equal(EditorTool.None, _viewModel.SelectedTool); // Tool resets after creation
    }

    [Fact]
    public void CreateObjectAtPosition_WithButtonTool_CreatesButtonObject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        _viewModel.SelectedTool = EditorTool.Button;

        // Act
        _viewModel.CreateObjectAtPosition(200, 300);

        // Assert
        Assert.Single(_viewModel.CurrentSlide!.Layers[0].Objects);
        var buttonObject = _viewModel.CurrentSlide.Layers[0].Objects[0] as ButtonObject;
        Assert.NotNull(buttonObject);
        Assert.Equal(200, buttonObject!.X);
        Assert.Equal(300, buttonObject.Y);
        Assert.Equal(150, buttonObject.Width);
        Assert.Equal(40, buttonObject.Height);
        Assert.Equal("Button", buttonObject.Label);
    }

    [Fact]
    public void CreateObjectAtPosition_NoTool_DoesNotCreateObject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        _viewModel.SelectedTool = EditorTool.None;

        // Act
        _viewModel.CreateObjectAtPosition(100, 200);

        // Assert
        Assert.Empty(_viewModel.CurrentSlide!.Layers[0].Objects);
    }

    [Fact]
    public void CreateObjectAtPosition_NoCurrentSlide_DoesNotCreateObject()
    {
        // Arrange
        _viewModel.SelectedTool = EditorTool.Text;

        // Act
        _viewModel.CreateObjectAtPosition(100, 200);

        // Assert
        Assert.Null(_viewModel.CurrentSlide);
    }

    [Fact]
    public void DeleteSelectedObject_WithSelectedObject_RemovesFromSlide()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;

        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        _viewModel.CurrentSlide!.Layers[0].Objects.Add(textObject);
        _viewModel.SelectedObject = textObject;

        // Act
        _viewModel.DeleteSelectedObject();

        // Assert
        Assert.Empty(_viewModel.CurrentSlide.Layers[0].Objects);
        Assert.Null(_viewModel.SelectedObject);
    }

    [Fact]
    public void DeleteSelectedObject_NoSelectedObject_DoesNothing()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;

        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        _viewModel.CurrentSlide!.Layers[0].Objects.Add(textObject);

        // Act
        _viewModel.DeleteSelectedObject();

        // Assert
        Assert.Single(_viewModel.CurrentSlide.Layers[0].Objects);
    }

    [Fact]
    public void AddSlide_AddsSlideToProject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        _viewModel.CurrentProject = project;
        var initialCount = project.Slides.Count;

        // Act
        _viewModel.AddSlideCommand.Execute(null);

        // Assert
        Assert.Equal(initialCount + 1, project.Slides.Count);
        Assert.NotNull(_viewModel.CurrentSlide);
        Assert.Contains(_viewModel.CurrentSlide, _viewModel.Slides);
    }

    [Fact]
    public void DeleteSlide_RemovesSlideFromProject()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide1 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 1" };
        var slide2 = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 2" };
        project.AddSlide(slide1);
        project.AddSlide(slide2);
        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = slide1;

        // Act
        _viewModel.DeleteSlideCommand.Execute(null);

        // Assert
        Assert.Single(project.Slides);
        Assert.Equal(slide2, _viewModel.CurrentSlide);
    }

    [Fact]
    public void DuplicateSlide_CreatesCopyWithObjects()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Original" };
        var layer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" };
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text", Text = "Hello" };
        layer.Objects.Add(textObject);
        slide.Layers.Add(layer);
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = slide;

        // Act
        _viewModel.DuplicateSlideCommand.Execute(null);

        // Assert
        Assert.Equal(2, project.Slides.Count);
        var duplicated = project.Slides[1];
        Assert.Equal("Original (Copy)", duplicated.Title);
        Assert.Single(duplicated.Layers[0].Objects);
        var copiedObject = duplicated.Layers[0].Objects[0] as TextObject;
        Assert.NotNull(copiedObject);
        Assert.Equal("Hello", copiedObject!.Text);
        Assert.NotEqual(textObject.Id, copiedObject.Id); // Different IDs
    }

    [Fact]
    public void AddLayer_AddsLayerToCurrentSlide()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        slide.Layers.Add(new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer" });
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;
        var initialLayerCount = slide.Layers.Count;

        // Act
        _viewModel.AddLayerCommand.Execute(null);

        // Assert
        Assert.Equal(initialLayerCount + 1, slide.Layers.Count);
        Assert.Equal(initialLayerCount + 1, _viewModel.Layers.Count);
    }

    [Fact]
    public void DeleteLayer_RemovesLayerFromSlide()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var layer1 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Layer 1" };
        var layer2 = new Layer { Id = Guid.NewGuid().ToString(), Name = "Layer 2" };
        slide.Layers.Add(layer1);
        slide.Layers.Add(layer2);
        project.AddSlide(slide);
        _viewModel.CurrentProject = project;

        // Act
        _viewModel.DeleteLayerCommand.Execute(layer1);

        // Assert
        Assert.Single(slide.Layers);
        Assert.Equal(layer2, slide.Layers[0]);
    }

    [Fact]
    public void SelectedTextObject_WithTextObjectSelected_ReturnsObject()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text", Text = "Hello" };

        // Act
        _viewModel.SelectedObject = textObject;

        // Assert
        Assert.Equal(textObject, _viewModel.SelectedTextObject);
        Assert.Null(_viewModel.SelectedImageObject);
        Assert.Null(_viewModel.SelectedButtonObject);
    }

    [Fact]
    public void SelectedImageObject_WithImageObjectSelected_ReturnsObject()
    {
        // Arrange
        var imageObject = new ImageObject { Id = Guid.NewGuid().ToString(), Name = "Image" };

        // Act
        _viewModel.SelectedObject = imageObject;

        // Assert
        Assert.Equal(imageObject, _viewModel.SelectedImageObject);
        Assert.Null(_viewModel.SelectedTextObject);
        Assert.Null(_viewModel.SelectedButtonObject);
    }

    [Fact]
    public void ToggleObjectTimeline_NoTimeline_CreatesTimeline()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        _viewModel.SelectedObject = textObject;

        // Act
        _viewModel.ToggleObjectTimelineCommand.Execute(null);

        // Assert
        Assert.NotNull(textObject.Timeline);
        Assert.Equal(0, textObject.Timeline!.StartTime);
        Assert.Equal(5.0, textObject.Timeline.Duration);
    }

    [Fact]
    public void ToggleObjectTimeline_WithTimeline_RemovesTimeline()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        textObject.Timeline = new Timeline { StartTime = 1.0, Duration = 3.0 };
        _viewModel.SelectedObject = textObject;

        // Act
        _viewModel.ToggleObjectTimelineCommand.Execute(null);

        // Assert
        Assert.Null(textObject.Timeline);
    }
}
