using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class VariableTests
{
    [Fact]
    public void Variable_Properties_CanBeSet()
    {
        // Arrange
        var variable = new Variable
        {
            Id = "var1",
            Name = "MyVariable",
            Type = VariableType.Boolean,
            DefaultValue = true
        };

        // Assert
        Assert.Equal("var1", variable.Id);
        Assert.Equal("MyVariable", variable.Name);
        Assert.Equal(VariableType.Boolean, variable.Type);
        Assert.Equal(true, variable.DefaultValue);
    }

    [Fact]
    public void Variable_CanHaveNumberType()
    {
        // Arrange
        var variable = new Variable
        {
            Id = "var2",
            Name = "Count",
            Type = VariableType.Number,
            DefaultValue = 42
        };

        // Assert
        Assert.Equal(VariableType.Number, variable.Type);
        Assert.Equal(42, variable.DefaultValue);
    }

    [Fact]
    public void Variable_CanHaveStringType()
    {
        // Arrange
        var variable = new Variable
        {
            Id = "var3",
            Name = "Message",
            Type = VariableType.String,
            DefaultValue = "Hello"
        };

        // Assert
        Assert.Equal(VariableType.String, variable.Type);
        Assert.Equal("Hello", variable.DefaultValue);
    }

    [Fact]
    public void Variable_DefaultValue_CanBeNull()
    {
        // Arrange
        var variable = new Variable
        {
            Id = "var4",
            Name = "NullableVar",
            Type = VariableType.String,
            DefaultValue = null
        };

        // Assert
        Assert.Null(variable.DefaultValue);
    }
}
