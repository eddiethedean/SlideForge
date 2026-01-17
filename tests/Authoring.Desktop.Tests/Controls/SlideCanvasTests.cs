using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Authoring.Core.Models;
using Authoring.Desktop.Tests.Helpers;
using Authoring.Desktop.Views.Controls;
using Xunit;

namespace Authoring.Desktop.Tests.Controls;

public class SlideCanvasTests : AvaloniaTestBase
{
    [AvaloniaFact]
    public void ZoomLevel_Default_IsOne()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(1.0, canvas.ZoomLevel);
    }

    [AvaloniaFact]
    public void ZoomLevel_SetValidValue_UpdatesCorrectly()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        canvas.ZoomLevel = 2.0;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(2.0, canvas.ZoomLevel);
    }

    [AvaloniaFact]
    public void ZoomLevel_SetZero_DoesNotUpdate()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;
        canvas.ZoomLevel = 0;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [AvaloniaFact]
    public void ZoomLevel_SetNegative_DoesNotUpdate()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;
        canvas.ZoomLevel = -1.0;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [AvaloniaFact]
    public void ZoomLevel_SetAboveMaximum_DoesNotUpdate()
    {
        // Arrange & Act & Assert
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;
        canvas.ZoomLevel = 10.0; // Above maximum of 5.0
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [AvaloniaFact]
    public void CurrentSlide_SetValue_UpdatesProperty()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Test Slide" };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.CurrentSlide = slide;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(slide, canvas.CurrentSlide);
    }

    [AvaloniaFact]
    public void CurrentSlide_SetNull_ClearsProperty()
    {
        // Arrange
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Test Slide" };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.CurrentSlide = slide;
        canvas.CurrentSlide = null;
        Dispatcher.UIThread.RunJobs();
        Assert.Null(canvas.CurrentSlide);
    }

    [AvaloniaFact]
    public void SelectedObject_SetValue_UpdatesProperty()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.SelectedObject = textObject;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(textObject, canvas.SelectedObject);
    }

    [AvaloniaFact]
    public void SelectedObject_SetNull_ClearsProperty()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.SelectedObject = textObject;
        canvas.SelectedObject = null;
        Dispatcher.UIThread.RunJobs();
        Assert.Null(canvas.SelectedObject);
    }

    [AvaloniaFact]
    public void ScreenToSlide_ConvertsCoordinates_WithZoomLevel()
    {
        // Arrange
        var screenPoint = new Point(240, 240); // Account for 20px margin, zoom 2.0

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.ZoomLevel = 2.0;
        Dispatcher.UIThread.RunJobs();
        var slidePoint = canvas.ScreenToSlide(screenPoint);
        // (240 - 20) / 2.0 = 110
        Assert.Equal(110, slidePoint.X, precision: 2);
        Assert.Equal(110, slidePoint.Y, precision: 2);
    }

    [AvaloniaFact]
    public void ScreenToSlide_WithDefaultZoom_AccountsForMargin()
    {
        // Arrange
        var screenPoint = new Point(120, 120); // 100px slide + 20px margin

        // Act & Assert
        var canvas = new SlideCanvas();
        Dispatcher.UIThread.RunJobs();
        var slidePoint = canvas.ScreenToSlide(screenPoint);
        Assert.Equal(100, slidePoint.X, precision: 2);
        Assert.Equal(100, slidePoint.Y, precision: 2);
    }

    [AvaloniaFact]
    public void ScreenToSlide_WithHalfZoom_CorrectlyScales()
    {
        // Arrange
        var screenPoint = new Point(70, 70); // (50px slide * 0.5) + 20px margin

        // Act & Assert
        var canvas = new SlideCanvas();
        canvas.ZoomLevel = 0.5;
        Dispatcher.UIThread.RunJobs();
        var slidePoint = canvas.ScreenToSlide(screenPoint);
        // (70 - 20) / 0.5 = 100
        Assert.Equal(100, slidePoint.X, precision: 2);
        Assert.Equal(100, slidePoint.Y, precision: 2);
    }

    [AvaloniaFact]
    public void SelectedObjectChanged_RaisesObjectSelectedEvent()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act & Assert
        SlideObject? eventObject = null;
        var canvas = new SlideCanvas();
        canvas.ObjectSelected += (sender, obj) => { eventObject = obj; };
        canvas.SelectedObject = textObject;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(textObject, eventObject);
    }

    [AvaloniaFact]
    public void SelectedObject_SetSameObject_DoesNotRaiseEventTwice()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act & Assert
        int eventCount = 0;
        var canvas = new SlideCanvas();
        canvas.ObjectSelected += (sender, obj) => { eventCount++; };
        canvas.SelectedObject = textObject;
        canvas.SelectedObject = textObject; // Set same object again
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(1, eventCount); // Should only fire once
    }
}
