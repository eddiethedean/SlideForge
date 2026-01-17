using Authoring.Desktop.Converters;
using Xunit;

namespace Authoring.Desktop.Tests.Converters;

public class InverseBoolConverterTests
{
    private readonly InverseBoolConverter _converter = new();

    [Fact]
    public void Convert_True_ReturnsFalse()
    {
        // Act
        var result = _converter.Convert(true, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_False_ReturnsTrue()
    {
        // Act
        var result = _converter.Convert(false, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonBoolValue_ReturnsOriginalValue()
    {
        // Act
        var stringResult = _converter.Convert("test", typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);
        var intResult = _converter.Convert(123, typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);
        var nullResult = _converter.Convert(null, typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test", stringResult);
        Assert.Equal(123, intResult);
        Assert.Null(nullResult);
    }

    [Fact]
    public void ConvertBack_True_ReturnsFalse()
    {
        // Act
        var result = _converter.ConvertBack(true, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_False_ReturnsTrue()
    {
        // Act
        var result = _converter.ConvertBack(false, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_NonBoolValue_ReturnsOriginalValue()
    {
        // Act
        var stringResult = _converter.ConvertBack("test", typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);
        var intResult = _converter.ConvertBack(123, typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);
        var nullResult = _converter.ConvertBack(null, typeof(object), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test", stringResult);
        Assert.Equal(123, intResult);
        Assert.Null(nullResult);
    }

    [Fact]
    public void Convert_RoundTrip_PreservesValue()
    {
        // Act
        var converted = _converter.Convert(true, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(bool), null, System.Globalization.CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, convertedBack);
    }
}
