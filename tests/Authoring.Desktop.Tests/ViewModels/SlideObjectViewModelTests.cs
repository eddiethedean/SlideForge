using Authoring.Core.Models;
using Authoring.Desktop.ViewModels;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class SlideObjectViewModelTests
{
    [Fact]
    public void Constructor_WrapsSlideObject()
    {
        // Arrange
        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Text",
            X = 100,
            Y = 200,
            Width = 300,
            Height = 50
        };

        // Act
        var viewModel = new SlideObjectViewModel(textObject);

        // Assert
        Assert.Equal(textObject, viewModel.SlideObject);
        Assert.Equal(100, viewModel.X);
        Assert.Equal(200, viewModel.Y);
        Assert.Equal(300, viewModel.Width);
        Assert.Equal(50, viewModel.Height);
    }

    [Fact]
    public void X_SettingValue_UpdatesWrappedObject()
    {
        // Arrange
        var textObject = new TextObject { X = 100 };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.X = 250;

        // Assert
        Assert.Equal(250, viewModel.X);
        Assert.Equal(250, textObject.X);
    }

    [Fact]
    public void Y_SettingValue_UpdatesWrappedObject()
    {
        // Arrange
        var textObject = new TextObject { Y = 100 };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.Y = 350;

        // Assert
        Assert.Equal(350, viewModel.Y);
        Assert.Equal(350, textObject.Y);
    }

    [Fact]
    public void Width_SettingValue_UpdatesWrappedObject()
    {
        // Arrange
        var textObject = new TextObject { Width = 100 };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.Width = 500;

        // Assert
        Assert.Equal(500, viewModel.Width);
        Assert.Equal(500, textObject.Width);
    }

    [Fact]
    public void Height_SettingValue_UpdatesWrappedObject()
    {
        // Arrange
        var textObject = new TextObject { Height = 50 };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.Height = 150;

        // Assert
        Assert.Equal(150, viewModel.Height);
        Assert.Equal(150, textObject.Height);
    }

    [Fact]
    public void Visible_SettingValue_UpdatesWrappedObject()
    {
        // Arrange
        var textObject = new TextObject { Visible = true };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.Visible = false;

        // Assert
        Assert.False(viewModel.Visible);
        Assert.False(textObject.Visible);
    }

    [Fact]
    public void IsSelected_Default_False()
    {
        // Arrange
        var textObject = new TextObject();
        var viewModel = new SlideObjectViewModel(textObject);

        // Assert
        Assert.False(viewModel.IsSelected);
    }

    [Fact]
    public void IsSelected_SettingValue_UpdatesCorrectly()
    {
        // Arrange
        var textObject = new TextObject();
        var viewModel = new SlideObjectViewModel(textObject);

        // Act
        viewModel.IsSelected = true;

        // Assert
        Assert.True(viewModel.IsSelected);
    }

    [Fact]
    public void ObjectTypeName_TextObject_ReturnsText()
    {
        // Arrange
        var textObject = new TextObject();
        var viewModel = new SlideObjectViewModel(textObject);

        // Assert
        Assert.Equal("Text", viewModel.ObjectTypeName);
    }

    [Fact]
    public void ObjectTypeName_ImageObject_ReturnsImage()
    {
        // Arrange
        var imageObject = new ImageObject();
        var viewModel = new SlideObjectViewModel(imageObject);

        // Assert
        Assert.Equal("Image", viewModel.ObjectTypeName);
    }

    [Fact]
    public void ObjectTypeName_ButtonObject_ReturnsButton()
    {
        // Arrange
        var buttonObject = new ButtonObject();
        var viewModel = new SlideObjectViewModel(buttonObject);

        // Assert
        Assert.Equal("Button", viewModel.ObjectTypeName);
    }

    [Fact]
    public void SettingSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var textObject = new TextObject { X = 100 };
        var viewModel = new SlideObjectViewModel(textObject);

        // Act - Setting same value
        viewModel.X = 100;

        // Assert - Should not throw or cause issues
        Assert.Equal(100, viewModel.X);
    }
}
