using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class LayerTests
{
    [Fact]
    public void Layer_InitializesWithDefaults()
    {
        // Act
        var layer = new Layer();

        // Assert
        Assert.NotNull(layer.Objects);
        Assert.Empty(layer.Objects);
        Assert.True(layer.Visible);
    }

    [Fact]
    public void Layer_Properties_CanBeSet()
    {
        // Arrange
        var layer = new Layer
        {
            Id = "layer1",
            Name = "Base Layer",
            Visible = false
        };

        // Assert
        Assert.Equal("layer1", layer.Id);
        Assert.Equal("Base Layer", layer.Name);
        Assert.False(layer.Visible);
    }

    [Fact]
    public void Layer_CanHaveObjects()
    {
        // Arrange
        var layer = new Layer { Id = "layer1", Name = "Test Layer" };
        var textObject = new TextObject { Id = "text1", Text = "Hello" };

        // Act
        layer.Objects.Add(textObject);

        // Assert
        Assert.Single(layer.Objects);
        Assert.Equal(textObject, layer.Objects[0]);
    }

    [Fact]
    public void Layer_CanHaveMultipleObjects()
    {
        // Arrange
        var layer = new Layer { Id = "layer1", Name = "Test Layer" };
        var textObject = new TextObject { Id = "text1" };
        var imageObject = new ImageObject { Id = "img1" };
        var buttonObject = new ButtonObject { Id = "btn1" };

        // Act
        layer.Objects.Add(textObject);
        layer.Objects.Add(imageObject);
        layer.Objects.Add(buttonObject);

        // Assert
        Assert.Equal(3, layer.Objects.Count);
    }

    [Fact]
    public void Layer_CanContainDifferentObjectTypes()
    {
        // Arrange
        var layer = new Layer { Id = "layer1", Name = "Mixed Layer" };
        
        // Act
        layer.Objects.Add(new TextObject { Id = "text1" });
        layer.Objects.Add(new ImageObject { Id = "img1" });
        layer.Objects.Add(new ButtonObject { Id = "btn1" });

        // Assert
        Assert.IsType<TextObject>(layer.Objects[0]);
        Assert.IsType<ImageObject>(layer.Objects[1]);
        Assert.IsType<ButtonObject>(layer.Objects[2]);
    }
}
