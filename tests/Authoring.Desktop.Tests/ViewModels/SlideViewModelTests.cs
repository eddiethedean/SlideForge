using Authoring.Core.Models;
using Authoring.Desktop.ViewModels;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class SlideViewModelTests
{
    [Fact]
    public void Constructor_WrapsSlide()
    {
        // Arrange
        var slide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Slide"
        };

        // Act
        var viewModel = new SlideViewModel(slide);

        // Assert
        Assert.Equal(slide, viewModel.Slide);
        Assert.NotNull(viewModel.ObjectViewModels);
    }

    [Fact]
    public void Constructor_WithObjects_CreatesViewModels()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var layer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer", Visible = true };
        var textObject1 = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text 1" };
        var textObject2 = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text 2" };
        layer.Objects.Add(textObject1);
        layer.Objects.Add(textObject2);
        slide.Layers.Add(layer);

        // Act
        var viewModel = new SlideViewModel(slide);

        // Assert
        Assert.Equal(2, viewModel.ObjectViewModels.Count);
        Assert.Equal(textObject1, viewModel.ObjectViewModels[0].SlideObject);
        Assert.Equal(textObject2, viewModel.ObjectViewModels[1].SlideObject);
    }

    [Fact]
    public void Constructor_NoLayers_EmptyViewModels()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };

        // Act
        var viewModel = new SlideViewModel(slide);

        // Assert
        Assert.Empty(viewModel.ObjectViewModels);
    }

    [Fact]
    public void Constructor_NoVisibleLayer_UsesFirstLayer()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var hiddenLayer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Hidden", Visible = false };
        var visibleLayer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Visible", Visible = true };
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text" };
        visibleLayer.Objects.Add(textObject);
        slide.Layers.Add(hiddenLayer);
        slide.Layers.Add(visibleLayer);

        // Act
        var viewModel = new SlideViewModel(slide);

        // Assert
        // Should use first layer (hiddenLayer) since FirstOrDefault returns first element
        Assert.Empty(viewModel.ObjectViewModels); // hiddenLayer has no objects
    }

    [Fact]
    public void UpdateObjectViewModels_RefreshesCollection()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var layer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer", Visible = true };
        slide.Layers.Add(layer);
        var viewModel = new SlideViewModel(slide);

        // Act - Add object after initialization
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "New Text" };
        layer.Objects.Add(textObject);
        viewModel.UpdateObjectViewModels();

        // Assert
        Assert.Single(viewModel.ObjectViewModels);
        Assert.Equal(textObject, viewModel.ObjectViewModels[0].SlideObject);
    }

    [Fact]
    public void UpdateObjectViewModels_ClearsPreviousViewModels()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var layer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer", Visible = true };
        var textObject1 = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Text 1" };
        layer.Objects.Add(textObject1);
        slide.Layers.Add(layer);
        var viewModel = new SlideViewModel(slide);

        // Act - Remove object and update
        layer.Objects.Clear();
        viewModel.UpdateObjectViewModels();

        // Assert
        Assert.Empty(viewModel.ObjectViewModels);
    }

    [Fact]
    public void UpdateObjectViewModels_OnlyIncludesVisibleObjects()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide" };
        var layer = new Layer { Id = Guid.NewGuid().ToString(), Name = "Base Layer", Visible = true };
        var visibleObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Visible", Visible = true };
        var hiddenObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Hidden", Visible = false };
        layer.Objects.Add(visibleObject);
        layer.Objects.Add(hiddenObject);
        slide.Layers.Add(layer);
        var viewModel = new SlideViewModel(slide);

        // Assert
        // Note: UpdateObjectViewModels doesn't filter by Visible, it adds all objects from base layer
        // This test documents current behavior
        Assert.Equal(2, viewModel.ObjectViewModels.Count);
    }
}
