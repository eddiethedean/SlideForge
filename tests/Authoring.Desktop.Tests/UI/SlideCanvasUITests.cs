using Avalonia.Headless;
using Authoring.Core.Models;
using Authoring.Desktop.Views.Controls;
using Xunit;

namespace Authoring.Desktop.Tests.UI;

[Trait("Category", "UI")]
public class SlideCanvasUITests
{
    [Fact]
    public void SlideCanvas_CanBeInstantiated()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        // Act
        var canvas = new SlideCanvas();

        // Assert
        Assert.NotNull(canvas);
    }

    [Fact]
    public void SlideCanvas_WithSlide_LoadsCorrectly()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var canvas = new SlideCanvas();
        var slide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Slide",
            Width = 1920,
            Height = 1080
        };

        // Act
        canvas.CurrentSlide = slide;

        // Assert
        Assert.Equal(slide, canvas.CurrentSlide);
    }

    [Fact]
    public void SlideCanvas_WithSelectedObject_LoadsCorrectly()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var canvas = new SlideCanvas();
        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Text = "Hello"
        };

        // Act
        canvas.SelectedObject = textObject;

        // Assert
        Assert.Equal(textObject, canvas.SelectedObject);
    }
}
