using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Tests.Extensions;

namespace Authoring.Core.Tests.Integration;

[Trait("Category", "Integration")]
public class SerializationIntegrationTests
{
    [Fact]
    public void Serialize_ComplexNestedStructure_PreservesAllData()
    {
        // Arrange
        var project = TestDataFactory.CreateComplexProject();

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        AssertExtensions.AssertProjectsEqual(project, deserialized, compareIds: true);
    }

    [Fact]
    public void Serialize_AllObjectTypes_PreservesTypeInformation()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("All Types")
            .WithSlide(s => s
                .WithTitle("All Objects")
                .WithObject(o => o.BuildTextObject("Text", fontSize: 20, color: "#FF0000"))
                .WithObject(o => o.BuildImageObject("image.png", maintainAspectRatio: false))
                .WithObject(o => o.BuildButtonObject("Click Me", enabled: false)))
            .Build();

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var slide = deserialized.Slides[0];
        var objects = slide.Layers[0].Objects;
        
        Assert.Contains(objects, o => o is TextObject);
        Assert.Contains(objects, o => o is ImageObject);
        Assert.Contains(objects, o => o is ButtonObject);

        var textObj = objects.OfType<TextObject>().First();
        Assert.Equal("Text", textObj.Text);
        Assert.Equal(20, textObj.FontSize);
        Assert.Equal("#FF0000", textObj.Color);

        var imageObj = objects.OfType<ImageObject>().First();
        Assert.Equal("image.png", imageObj.SourcePath);
        Assert.False(imageObj.MaintainAspectRatio);

        var buttonObj = objects.OfType<ButtonObject>().First();
        Assert.Equal("Click Me", buttonObj.Label);
        Assert.False(buttonObj.Enabled);
    }

    [Fact]
    public void Serialize_AllVariableTypes_PreservesTypeAndValue()
    {
        // Arrange
        var project = TestDataFactory.CreateProjectWithAllVariableTypes();

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Equal(3, deserialized.Variables.Count);
        
        var boolVar = deserialized.Variables.First(v => v.Type == VariableType.Boolean);
        Assert.Equal(true, boolVar.DefaultValue);

        var numVar = deserialized.Variables.First(v => v.Type == VariableType.Number);
        Assert.Equal(42, numVar.DefaultValue);

        var strVar = deserialized.Variables.First(v => v.Type == VariableType.String);
        Assert.Equal("Hello", strVar.DefaultValue);
    }

    [Fact]
    public void Serialize_AllActionTypes_PreservesActionData()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("All Actions")
            .WithVariable("test", "Test", VariableType.Number, 0)
            .WithSlide(s => s
                .WithId("slide1")
                .WithTitle("Slide 1"))
            .WithSlide(s => s
                .WithId("slide2")
                .WithTitle("Slide 2")
                .WithLayer("layer1", "Layer 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Test Actions")))
            .Build();

        Assert.Equal(2, project.Slides.Count);
        Assert.NotEmpty(project.Slides[1].Layers);
        var buttons = project.Slides[1].Layers[0].Objects.OfType<ButtonObject>().ToList();
        Assert.NotEmpty(buttons);
        var button = buttons.First();
        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide1" },
                new SetVariableAction { VariableId = "test", Value = 100 },
                new ShowLayerAction { LayerId = "layer1" },
                new HideLayerAction { LayerId = "layer1" }
            }
        });

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var loadedButton = deserialized.Slides[1].Layers[0].Objects.OfType<ButtonObject>().First();
        var trigger = loadedButton.Triggers.First();
        Assert.Equal(4, trigger.Actions.Count);
        
        Assert.Contains(trigger.Actions, a => a is NavigateToSlideAction);
        Assert.Contains(trigger.Actions, a => a is SetVariableAction);
        Assert.Contains(trigger.Actions, a => a is ShowLayerAction);
        Assert.Contains(trigger.Actions, a => a is HideLayerAction);
    }

    [Fact]
    public void Serialize_UnicodeCharacters_PreservesCorrectly()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("测试项目")
            .WithAuthor("Автор")
            .WithSlide(s => s
                .WithTitle("日本語スライド")
                .WithObject(o => o
                    .BuildTextObject("Hello 世界")))
            .Build();

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Equal("测试项目", deserialized.Name);
        Assert.Equal("Автор", deserialized.Author);
        Assert.Equal("日本語スライド", deserialized.Slides[0].Title);
        
        Assert.NotEmpty(deserialized.Slides[0].Layers);
        var textObjects = deserialized.Slides[0].Layers[0].Objects.OfType<TextObject>().ToList();
        Assert.NotEmpty(textObjects);
        var textObj = textObjects.First();
        Assert.Equal("Hello 世界", textObj.Text);
    }

    [Fact]
    public void Serialize_LargeProject_PerformsAcceptably()
    {
        // Arrange
        var project = TestDataFactory.CreateLargeProject(slides: 50, objectsPerSlide: 20);

        // Act
        var startTime = DateTime.UtcNow;
        var json = ProjectJsonSerializer.Serialize(project);
        var serializeTime = DateTime.UtcNow - startTime;

        startTime = DateTime.UtcNow;
        var deserialized = ProjectJsonSerializer.Deserialize(json);
        var deserializeTime = DateTime.UtcNow - startTime;

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(50, deserialized.Slides.Count);
        // Performance assertions (should complete in reasonable time)
        Assert.True(serializeTime.TotalSeconds < 5, "Serialization took too long");
        Assert.True(deserializeTime.TotalSeconds < 5, "Deserialization took too long");
    }

    [Fact]
    public void Serialize_RoundTrip_MaintainsAllReferences()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("References")
            .WithVariable("var1", "Variable 1", VariableType.Number, 0)
            .WithSlide(s => s
                .WithId("slide1")
                .WithTitle("Slide 1"))
            .WithSlide(s => s
                .WithId("slide2")
                .WithTitle("Slide 2")
                .WithObject(o => o
                    .BuildButtonObject("Navigate")))
            .Build();

        // Add trigger that references slide and variable
        Assert.Equal(2, project.Slides.Count);
        Assert.NotEmpty(project.Slides[1].Layers);
        var buttons = project.Slides[1].Layers[0].Objects.OfType<ButtonObject>().ToList();
        Assert.NotEmpty(buttons);
        var button = buttons.First();
        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide1" },
                new SetVariableAction { VariableId = "var1", Value = 5 }
            }
        });

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert - Verify references are preserved
        Assert.Equal(2, deserialized.Slides.Count);
        Assert.NotEmpty(deserialized.Slides[1].Layers);
        var loadedButtons = deserialized.Slides[1].Layers[0].Objects.OfType<ButtonObject>().ToList();
        Assert.NotEmpty(loadedButtons);
        var loadedButton = loadedButtons.First();
        Assert.NotEmpty(loadedButton.Triggers);
        var navActions = loadedButton.Triggers[0].Actions.OfType<NavigateToSlideAction>().ToList();
        Assert.NotEmpty(navActions);
        var navAction = navActions.First();
        Assert.Equal("slide1", navAction.TargetSlideId);
        
        var setActions = loadedButton.Triggers[0].Actions.OfType<SetVariableAction>().ToList();
        Assert.NotEmpty(setActions);
        var setAction = setActions.First();
        Assert.Equal("var1", setAction.VariableId);

        // Verify referenced entities exist
        Assert.Contains(deserialized.Slides, s => s.Id == "slide1");
        Assert.Contains(deserialized.Variables, v => v.Id == "var1");
    }
}
