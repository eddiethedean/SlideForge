using Authoring.Core.Models;
using Authoring.Desktop.Views.Controls;
using Avalonia;
using Xunit;

namespace Authoring.Desktop.Tests.Controls;

public class SlideCanvasTests
{
    [Fact]
    public void ZoomLevel_Default_IsOne()
    {
        // Arrange & Act
        var canvas = new SlideCanvas();

        // Assert
        Assert.Equal(1.0, canvas.ZoomLevel);
    }

    [Fact]
    public void ZoomLevel_SetValidValue_UpdatesCorrectly()
    {
        // Arrange
        var canvas = new SlideCanvas();

        // Act
        canvas.ZoomLevel = 2.0;

        // Assert
        Assert.Equal(2.0, canvas.ZoomLevel);
    }

    [Fact]
    public void ZoomLevel_SetZero_DoesNotUpdate()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;

        // Act
        canvas.ZoomLevel = 0;

        // Assert
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [Fact]
    public void ZoomLevel_SetNegative_DoesNotUpdate()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;

        // Act
        canvas.ZoomLevel = -1.0;

        // Assert
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [Fact]
    public void ZoomLevel_SetAboveMaximum_DoesNotUpdate()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var originalZoom = canvas.ZoomLevel;

        // Act
        canvas.ZoomLevel = 10.0; // Above maximum of 5.0

        // Assert
        Assert.Equal(originalZoom, canvas.ZoomLevel);
    }

    [Fact]
    public void CurrentSlide_SetValue_UpdatesProperty()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Test Slide" };

        // Act
        canvas.CurrentSlide = slide;

        // Assert
        Assert.Equal(slide, canvas.CurrentSlide);
    }

    [Fact]
    public void CurrentSlide_SetNull_ClearsProperty()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Test Slide" };
        canvas.CurrentSlide = slide;

        // Act
        canvas.CurrentSlide = null;

        // Assert
        Assert.Null(canvas.CurrentSlide);
    }

    [Fact]
    public void SelectedObject_SetValue_UpdatesProperty()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        canvas.SelectedObject = textObject;

        // Assert
        Assert.Equal(textObject, canvas.SelectedObject);
    }

    [Fact]
    public void SelectedObject_SetNull_ClearsProperty()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };
        canvas.SelectedObject = textObject;

        // Act
        canvas.SelectedObject = null;

        // Assert
        Assert.Null(canvas.SelectedObject);
    }

    [Fact]
    public void ScreenToSlide_ConvertsCoordinates_WithZoomLevel()
    {
        // Arrange
        var canvas = new SlideCanvas();
        canvas.ZoomLevel = 2.0;
        var screenPoint = new Point(240, 240); // Account for 20px margin, zoom 2.0

        // Act
        var slidePoint = canvas.ScreenToSlide(screenPoint);

        // Assert
        // (240 - 20) / 2.0 = 110
        Assert.Equal(110, slidePoint.X, precision: 2);
        Assert.Equal(110, slidePoint.Y, precision: 2);
    }

    [Fact]
    public void ScreenToSlide_WithDefaultZoom_AccountsForMargin()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var screenPoint = new Point(120, 120); // 100px slide + 20px margin

        // Act
        var slidePoint = canvas.ScreenToSlide(screenPoint);

        // Assert
        Assert.Equal(100, slidePoint.X, precision: 2);
        Assert.Equal(100, slidePoint.Y, precision: 2);
    }

    [Fact]
    public void ScreenToSlide_WithHalfZoom_CorrectlyScales()
    {
        // Arrange
        var canvas = new SlideCanvas();
        canvas.ZoomLevel = 0.5;
        var screenPoint = new Point(70, 70); // (50px slide * 0.5) + 20px margin

        // Act
        var slidePoint = canvas.ScreenToSlide(screenPoint);

        // Assert
        // (70 - 20) / 0.5 = 100
        Assert.Equal(100, slidePoint.X, precision: 2);
        Assert.Equal(100, slidePoint.Y, precision: 2);
    }

    [Fact]
    public void SelectedObjectChanged_RaisesObjectSelectedEvent()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };
        SlideObject? eventObject = null;
        canvas.ObjectSelected += (sender, obj) => { eventObject = obj; };

        // Act
        canvas.SelectedObject = textObject;

        // Assert
        Assert.Equal(textObject, eventObject);
    }

    [Fact]
    public void SelectedObject_SetSameObject_DoesNotRaiseEventTwice()
    {
        // Arrange
        var canvas = new SlideCanvas();
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };
        int eventCount = 0;
        canvas.ObjectSelected += (sender, obj) => { eventCount++; };

        // Act
        canvas.SelectedObject = textObject;
        canvas.SelectedObject = textObject; // Set same object again

        // Assert
        Assert.Equal(1, eventCount); // Should only fire once
    }
}
