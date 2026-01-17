using Authoring.Desktop.Converters;
using Xunit;

namespace Authoring.Desktop.Tests.Converters;

public class NotNullToBoolConverterTests
{
    private readonly NotNullToBoolConverter _converter = NotNullToBoolConverter.Instance;

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        // Act
        var result = _converter.Convert(null, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonNullValue_ReturnsTrue()
    {
        // Act
        var result = _converter.Convert("test", typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithInverseParameter_ReturnsInverse()
    {
        // Act
        var nullResult = _converter.Convert(null, typeof(bool), "inverse", System.Globalization.CultureInfo.InvariantCulture);
        var nonNullResult = _converter.Convert("test", typeof(bool), "inverse", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, nullResult); // null with inverse = true
        Assert.Equal(false, nonNullResult); // non-null with inverse = false
    }

    [Fact]
    public void Convert_WithNonInverseParameter_ReturnsNormal()
    {
        // Act
        var nullResult = _converter.Convert(null, typeof(bool), "other", System.Globalization.CultureInfo.InvariantCulture);
        var nonNullResult = _converter.Convert("test", typeof(bool), "other", System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, nullResult);
        Assert.Equal(true, nonNullResult);
    }

    [Fact]
    public void Convert_VariousObjectTypes_HandlesCorrectly()
    {
        // Test with different object types
        Assert.Equal(true, _converter.Convert(new object(), typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(true, _converter.Convert(123, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(true, _converter.Convert(true, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(true, _converter.Convert(string.Empty, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture)); // Empty string is not null
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(true, typeof(object), null, System.Globalization.CultureInfo.InvariantCulture));
    }
}
