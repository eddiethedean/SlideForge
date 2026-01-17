using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Authoring.Desktop.Views;
using Authoring.Desktop.Views.Controls;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.Integration;

[Trait("Category", "Integration")]
public class ObjectDraggingIntegrationTests
{
    [Fact]
    public void DragObject_UpdatesViewModelProperties_AndMarksProjectAsModified()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects.First();
        viewModel.SelectedObject = textObject;
        
        var initialModified = viewModel.IsModified;
        var initialX = textObject.X;
        var initialY = textObject.Y;

        // Act - Simulate dragging object
        textObject.X = 200;
        textObject.Y = 200;
        viewModel.OnObjectPropertyChanged(); // Simulates what happens during drag

        // Assert
        Assert.Equal(200, textObject.X);
        Assert.Equal(200, textObject.Y);
        Assert.Equal(200, viewModel.SelectedObject.X);
        Assert.Equal(200, viewModel.SelectedObject.Y);
        Assert.True(viewModel.IsModified);
    }

    [Fact]
    public void DragObject_PositionChange_ReflectedInPropertiesPanel()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects.First();
        viewModel.SelectedObject = textObject;

        // Act - Drag object to new position
        textObject.X = 300;
        textObject.Y = 400;

        // Assert - ViewModel should reflect the new position
        Assert.Equal(300, viewModel.SelectedObject.X);
        Assert.Equal(400, viewModel.SelectedObject.Y);
    }

    [Fact]
    public void DragObject_MultipleObjects_OnlySelectedObjectMoves()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        
        // Create first object
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        var object1 = viewModel.CurrentSlide!.Layers[0].Objects[0];
        
        // Create second object
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(300, 300);
        var object2 = viewModel.CurrentSlide.Layers[0].Objects[1];
        
        var object2OriginalX = object2.X;
        var object2OriginalY = object2.Y;

        // Act - Select and drag object1
        viewModel.SelectedObject = object1;
        object1.X = 200;
        object1.Y = 200;
        viewModel.OnObjectPropertyChanged();

        // Assert - Only object1 moved
        Assert.Equal(200, object1.X);
        Assert.Equal(200, object1.Y);
        Assert.Equal(object2OriginalX, object2.X);
        Assert.Equal(object2OriginalY, object2.Y);
    }

    [Fact]
    public void DragObject_WithinBounds_ProjectCanBeSaved()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects.First();
        viewModel.SelectedObject = textObject;

        // Act - Drag object within bounds
        textObject.X = 500;
        textObject.Y = 600;
        viewModel.OnObjectPropertyChanged();

        // Assert - Object is still valid and can be saved
        Assert.True(textObject.X >= 0);
        Assert.True(textObject.Y >= 0);
        Assert.True(textObject.X + textObject.Width <= viewModel.CurrentSlide.Width);
        Assert.True(textObject.Y + textObject.Height <= viewModel.CurrentSlide.Height);
        Assert.True(viewModel.IsModified);
    }

    [Fact]
    public void DragObject_AllObjectTypes_UpdateCorrectly()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;

        // Create text object
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects[0] as TextObject;
        viewModel.SelectedObject = textObject;

        // Act & Assert - Drag text object
        textObject!.X = 200;
        textObject.Y = 200;
        viewModel.OnObjectPropertyChanged();
        Assert.Equal(200, textObject.X);
        Assert.Equal(200, textObject.Y);
        Assert.NotNull(viewModel.SelectedObject);
        Assert.Equal(200, viewModel.SelectedObject!.X);
        Assert.Equal(200, viewModel.SelectedObject.Y);

        // Create button object
        viewModel.SelectedTool = EditorTool.Button;
        viewModel.CreateObjectAtPosition(300, 100);
        var buttonObject = viewModel.CurrentSlide.Layers[0].Objects[1] as ButtonObject;
        viewModel.SelectedObject = buttonObject;

        // Act & Assert - Drag button object  
        var buttonNewX = 400.0;
        var buttonNewY = 300.0;
        buttonObject!.X = buttonNewX;
        buttonObject.Y = buttonNewY;
        viewModel.OnObjectPropertyChanged();
        Assert.Equal(buttonNewX, buttonObject.X);
        Assert.Equal(buttonNewY, buttonObject.Y);
        Assert.NotNull(viewModel.SelectedObject);
        Assert.Equal(buttonNewX, viewModel.SelectedObject!.X);
        Assert.Equal(buttonNewY, viewModel.SelectedObject.Y);

        // Create image object
        viewModel.SelectedTool = EditorTool.Image;
        viewModel.CreateObjectAtPosition(500, 100);
        var imageObject = viewModel.CurrentSlide.Layers[0].Objects[2] as ImageObject;
        viewModel.SelectedObject = imageObject;

        // Act & Assert - Drag image object
        var imageNewX = 600.0;
        var imageNewY = 400.0;
        imageObject!.X = imageNewX;
        imageObject.Y = imageNewY;
        viewModel.OnObjectPropertyChanged();
        Assert.Equal(imageNewX, imageObject.X);
        Assert.Equal(imageNewY, imageObject.Y);
        Assert.NotNull(viewModel.SelectedObject);
        Assert.Equal(imageNewX, viewModel.SelectedObject!.X);
        Assert.Equal(imageNewY, viewModel.SelectedObject.Y);
    }

    [Fact]
    public void DragObject_ThenChangeSlide_PositionPreserved()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Slide 1").WithDimensions(1920, 1080))
            .WithSlide(s => s.WithTitle("Slide 2").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.CurrentSlide = project.Slides[0];
        
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects.First();
        
        // Act - Drag object
        textObject.X = 300;
        textObject.Y = 400;
        viewModel.OnObjectPropertyChanged();
        
        // Switch to another slide
        viewModel.CurrentSlide = project.Slides[1];
        
        // Switch back
        viewModel.CurrentSlide = project.Slides[0];
        var retrievedObject = viewModel.CurrentSlide.Layers[0].Objects.First();

        // Assert - Position should be preserved
        Assert.Equal(300, retrievedObject.X);
        Assert.Equal(400, retrievedObject.Y);
    }

    [Fact]
    public void DragObject_Serialization_RoundTripPreservesPosition()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project = ProjectBuilder.Create()
            .WithName("Drag Test")
            .WithSlide(s => s.WithTitle("Test Slide").WithDimensions(1920, 1080))
            .Build();

        viewModel.CurrentProject = project;
        viewModel.SelectedTool = EditorTool.Text;
        viewModel.CreateObjectAtPosition(100, 100);
        
        var textObject = viewModel.CurrentSlide!.Layers[0].Objects.First();

        // Act - Drag object to new position
        textObject.X = 500;
        textObject.Y = 600;
        viewModel.OnObjectPropertyChanged();

        // Serialize and deserialize
        var json = Authoring.Core.Serialization.ProjectJsonSerializer.Serialize(project);
        var deserialized = Authoring.Core.Serialization.ProjectJsonSerializer.Deserialize(json);

        // Assert - Position should be preserved after round-trip
        var deserializedObject = deserialized!.Slides[0].Layers[0].Objects.First();
        Assert.Equal(500, deserializedObject.X);
        Assert.Equal(600, deserializedObject.Y);
    }
}
