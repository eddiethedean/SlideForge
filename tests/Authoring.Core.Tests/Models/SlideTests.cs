using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class SlideTests
{
    [Fact]
    public void Slide_InitializesWithDefaults()
    {
        // Act
        var slide = new Slide();

        // Assert
        Assert.NotNull(slide.Layers);
        Assert.Empty(slide.Layers);
        Assert.Equal(1920, slide.Width);
        Assert.Equal(1080, slide.Height);
    }

    [Fact]
    public void Slide_Properties_CanBeSet()
    {
        // Arrange
        var slide = new Slide
        {
            Id = "slide1",
            Title = "Introduction",
            Width = 1280,
            Height = 720
        };

        // Assert
        Assert.Equal("slide1", slide.Id);
        Assert.Equal("Introduction", slide.Title);
        Assert.Equal(1280, slide.Width);
        Assert.Equal(720, slide.Height);
    }

    [Fact]
    public void Slide_CanHaveLayers()
    {
        // Arrange
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base Layer" };

        // Act
        slide.Layers.Add(layer);

        // Assert
        Assert.Single(slide.Layers);
        Assert.Equal(layer, slide.Layers[0]);
    }

    [Fact]
    public void Slide_CanHaveMultipleLayers()
    {
        // Arrange
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer1 = new Layer { Id = "layer1", Name = "Layer 1" };
        var layer2 = new Layer { Id = "layer2", Name = "Layer 2" };

        // Act
        slide.Layers.Add(layer1);
        slide.Layers.Add(layer2);

        // Assert
        Assert.Equal(2, slide.Layers.Count);
    }
}
