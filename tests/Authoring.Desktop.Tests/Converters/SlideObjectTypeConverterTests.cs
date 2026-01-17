using Authoring.Core.Models;
using Authoring.Desktop.Converters;
using Xunit;

namespace Authoring.Desktop.Tests.Converters;

public class SlideObjectTypeConverterTests
{
    private readonly SlideObjectTypeConverter _converter = SlideObjectTypeConverter.Instance;

    [Fact]
    public void Convert_TextObjectWithTextParameter_ReturnsTrue()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        var result = _converter.Convert(textObject, typeof(bool), "Text", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_TextObjectWithImageParameter_ReturnsFalse()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        var result = _converter.Convert(textObject, typeof(bool), "Image", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_ImageObjectWithImageParameter_ReturnsTrue()
    {
        // Arrange
        var imageObject = new ImageObject { Id = Guid.NewGuid().ToString(), Name = "Test", SourcePath = "image.png" };

        // Act
        var result = _converter.Convert(imageObject, typeof(bool), "Image", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_ImageObjectWithButtonParameter_ReturnsFalse()
    {
        // Arrange
        var imageObject = new ImageObject { Id = Guid.NewGuid().ToString(), Name = "Test", SourcePath = "image.png" };

        // Act
        var result = _converter.Convert(imageObject, typeof(bool), "Button", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_ButtonObjectWithButtonParameter_ReturnsTrue()
    {
        // Arrange
        var buttonObject = new ButtonObject { Id = Guid.NewGuid().ToString(), Name = "Test", Label = "Click Me" };

        // Act
        var result = _converter.Convert(buttonObject, typeof(bool), "Button", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_ButtonObjectWithTextParameter_ReturnsFalse()
    {
        // Arrange
        var buttonObject = new ButtonObject { Id = Guid.NewGuid().ToString(), Name = "Test", Label = "Click Me" };

        // Act
        var result = _converter.Convert(buttonObject, typeof(bool), "Text", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonSlideObject_ReturnsFalse()
    {
        // Act
        var result = _converter.Convert("not a slide object", typeof(bool), "Text", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        // Act
        var result = _converter.Convert(null, typeof(bool), "Text", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_InvalidTypeParameter_ReturnsFalse()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        var result = _converter.Convert(textObject, typeof(bool), "InvalidType", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonStringParameter_ReturnsFalse()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        var result = _converter.Convert(textObject, typeof(bool), 123, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NullParameter_ReturnsFalse()
    {
        // Arrange
        var textObject = new TextObject { Id = Guid.NewGuid().ToString(), Name = "Test", Text = "Hello" };

        // Act
        var result = _converter.Convert(textObject, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(true, typeof(SlideObject), null, System.Globalization.CultureInfo.InvariantCulture));
    }
}
