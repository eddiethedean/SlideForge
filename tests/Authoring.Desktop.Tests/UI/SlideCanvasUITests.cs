using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Authoring.Core.Models;
using Authoring.Desktop.Tests.Helpers;
using Authoring.Desktop.Views.Controls;
using Xunit;

namespace Authoring.Desktop.Tests.UI;

[Trait("Category", "UI")]
public class SlideCanvasUITests : AvaloniaTestBase
{
    [AvaloniaFact]
    public void SlideCanvas_CanBeInstantiated()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        Dispatcher.UIThread.RunJobs();
        Assert.NotNull(canvas);
    }

    [AvaloniaFact]
    public void SlideCanvas_WithSlide_LoadsCorrectly()
    {
        // Arrange
        var slide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Slide",
            Width = 1920,
            Height = 1080
        };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.CurrentSlide = slide;
        Dispatcher.UIThread.RunJobs();
        
        Assert.NotNull(canvas);
        Assert.Equal(slide, canvas.CurrentSlide);
    }

    [AvaloniaFact]
    public void SlideCanvas_WithSelectedObject_LoadsCorrectly()
    {
        // Arrange
        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Text = "Hello"
        };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.SelectedObject = textObject;
        Dispatcher.UIThread.RunJobs();
        
        Assert.NotNull(canvas);
        Assert.Equal(textObject, canvas.SelectedObject);
    }
}
