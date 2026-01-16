using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class SlideObjectTests
{
    [Fact]
    public void TextObject_InitializesWithDefaults()
    {
        // Act
        var textObject = new TextObject();

        // Assert
        Assert.True(textObject.Visible);
        Assert.NotNull(textObject.Triggers);
        Assert.Empty(textObject.Triggers);
        Assert.Equal("Arial", textObject.FontFamily);
        Assert.Equal(12.0, textObject.FontSize);
        Assert.Equal("#000000", textObject.Color);
    }

    [Fact]
    public void TextObject_Properties_CanBeSet()
    {
        // Arrange
        var textObject = new TextObject
        {
            Id = "text1",
            Name = "My Text",
            X = 100,
            Y = 200,
            Width = 300,
            Height = 50,
            Text = "Hello World",
            FontFamily = "Times New Roman",
            FontSize = 18.0,
            Color = "#FF0000"
        };

        // Assert
        Assert.Equal("text1", textObject.Id);
        Assert.Equal("My Text", textObject.Name);
        Assert.Equal(100, textObject.X);
        Assert.Equal(200, textObject.Y);
        Assert.Equal(300, textObject.Width);
        Assert.Equal(50, textObject.Height);
        Assert.Equal("Hello World", textObject.Text);
        Assert.Equal("Times New Roman", textObject.FontFamily);
        Assert.Equal(18.0, textObject.FontSize);
        Assert.Equal("#FF0000", textObject.Color);
    }

    [Fact]
    public void ImageObject_InitializesWithDefaults()
    {
        // Act
        var imageObject = new ImageObject();

        // Assert
        Assert.True(imageObject.Visible);
        Assert.True(imageObject.MaintainAspectRatio);
        Assert.NotNull(imageObject.Triggers);
        Assert.Empty(imageObject.Triggers);
    }

    [Fact]
    public void ImageObject_Properties_CanBeSet()
    {
        // Arrange
        var imageObject = new ImageObject
        {
            Id = "img1",
            Name = "My Image",
            X = 50,
            Y = 50,
            Width = 200,
            Height = 150,
            SourcePath = "/path/to/image.png",
            MaintainAspectRatio = false
        };

        // Assert
        Assert.Equal("img1", imageObject.Id);
        Assert.Equal("/path/to/image.png", imageObject.SourcePath);
        Assert.False(imageObject.MaintainAspectRatio);
    }

    [Fact]
    public void ButtonObject_InitializesWithDefaults()
    {
        // Act
        var buttonObject = new ButtonObject();

        // Assert
        Assert.True(buttonObject.Visible);
        Assert.True(buttonObject.Enabled);
        Assert.NotNull(buttonObject.Triggers);
        Assert.Empty(buttonObject.Triggers);
    }

    [Fact]
    public void ButtonObject_Properties_CanBeSet()
    {
        // Arrange
        var buttonObject = new ButtonObject
        {
            Id = "btn1",
            Name = "My Button",
            X = 400,
            Y = 300,
            Width = 150,
            Height = 40,
            Label = "Click Me",
            Enabled = false
        };

        // Assert
        Assert.Equal("btn1", buttonObject.Id);
        Assert.Equal("Click Me", buttonObject.Label);
        Assert.False(buttonObject.Enabled);
    }

    [Fact]
    public void SlideObject_CanHaveTimeline()
    {
        // Arrange
        var textObject = new TextObject { Id = "text1" };
        var timeline = new Timeline { StartTime = 1.5, Duration = 3.0 };

        // Act
        textObject.Timeline = timeline;

        // Assert
        Assert.NotNull(textObject.Timeline);
        Assert.Equal(1.5, textObject.Timeline.StartTime);
        Assert.Equal(3.0, textObject.Timeline.Duration);
    }

    [Fact]
    public void SlideObject_CanHaveTriggers()
    {
        // Arrange
        var buttonObject = new ButtonObject { Id = "btn1" };
        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action> { new NavigateToSlideAction { TargetSlideId = "slide2" } }
        };

        // Act
        buttonObject.Triggers.Add(trigger);

        // Assert
        Assert.Single(buttonObject.Triggers);
        Assert.Equal(trigger, buttonObject.Triggers[0]);
    }

    [Fact]
    public void SlideObject_Visible_CanBeSetToFalse()
    {
        // Arrange
        var textObject = new TextObject();

        // Act
        textObject.Visible = false;

        // Assert
        Assert.False(textObject.Visible);
    }
}
