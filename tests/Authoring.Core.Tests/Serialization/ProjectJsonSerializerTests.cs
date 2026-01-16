using Authoring.Core.Models;
using Authoring.Core.Serialization;

namespace Authoring.Core.Tests.Serialization;

public class ProjectJsonSerializerTests
{
    [Fact]
    public void Serialize_ProjectWithBasicProperties_ReturnsValidJson()
    {
        // Arrange
        var project = new Project
        {
            Name = "Test Project",
            Version = "1.0.0",
            Author = "Test Author"
        };

        // Act
        var json = ProjectJsonSerializer.Serialize(project);

        // Assert
        Assert.NotEmpty(json);
        Assert.Contains("Test Project", json);
        Assert.Contains("1.0.0", json);
        Assert.Contains("Test Author", json);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTripPreservesData()
    {
        // Arrange
        var original = new Project
        {
            Name = "Round Trip Test",
            Version = "2.0.0",
            Author = "Test Author"
        };
        original.AddSlide(new Slide { Id = "slide1", Title = "First Slide" });
        original.AddVariable(new Variable { Id = "var1", Name = "MyVar", Type = VariableType.Boolean, DefaultValue = true });

        // Act
        var json = ProjectJsonSerializer.Serialize(original);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.Version, deserialized.Version);
        Assert.Equal(original.Author, deserialized.Author);
        Assert.Equal(original.Slides.Count, deserialized.Slides.Count);
        Assert.Equal(original.Slides[0].Title, deserialized.Slides[0].Title);
        Assert.Equal(original.Variables.Count, deserialized.Variables.Count);
        Assert.Equal(original.Variables[0].Name, deserialized.Variables[0].Name);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesSlideObjects()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test Slide" };
        var layer = new Layer { Id = "layer1", Name = "Base Layer" };
        var textObject = new TextObject
        {
            Id = "text1",
            Name = "My Text",
            X = 100,
            Y = 200,
            Text = "Hello World",
            FontSize = 16.0
        };
        layer.Objects.Add(textObject);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var deserializedObject = deserialized.Slides[0].Layers[0].Objects[0];
        Assert.IsType<TextObject>(deserializedObject);
        var deserializedText = (TextObject)deserializedObject;
        Assert.Equal("text1", deserializedText.Id);
        Assert.Equal("Hello World", deserializedText.Text);
        Assert.Equal(100, deserializedText.X);
        Assert.Equal(200, deserializedText.Y);
        Assert.Equal(16.0, deserializedText.FontSize);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesPolymorphicSlideObjects()
    {
        // Arrange
        var project = new Project { Name = "Polymorphic Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        
        layer.Objects.Add(new TextObject { Id = "text1", Text = "Text" });
        layer.Objects.Add(new ImageObject { Id = "img1", SourcePath = "/image.png" });
        layer.Objects.Add(new ButtonObject { Id = "btn1", Label = "Click" });
        
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var objects = deserialized.Slides[0].Layers[0].Objects;
        Assert.Equal(3, objects.Count);
        Assert.IsType<TextObject>(objects[0]);
        Assert.IsType<ImageObject>(objects[1]);
        Assert.IsType<ButtonObject>(objects[2]);
        
        Assert.Equal("Text", ((TextObject)objects[0]).Text);
        Assert.Equal("/image.png", ((ImageObject)objects[1]).SourcePath);
        Assert.Equal("Click", ((ButtonObject)objects[2]).Label);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesPolymorphicActions()
    {
        // Arrange
        var project = new Project { Name = "Actions Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1", Label = "Button" };
        
        var trigger = new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" },
                new SetVariableAction { VariableId = "var1", Value = 42 },
                new ShowLayerAction { LayerId = "layer2" },
                new HideLayerAction { LayerId = "layer3" }
            }
        };
        button.Triggers.Add(trigger);
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var actions = deserialized.Slides[0].Layers[0].Objects[0].Triggers[0].Actions;
        Assert.Equal(4, actions.Count);
        Assert.IsType<NavigateToSlideAction>(actions[0]);
        Assert.IsType<SetVariableAction>(actions[1]);
        Assert.IsType<ShowLayerAction>(actions[2]);
        Assert.IsType<HideLayerAction>(actions[3]);
        
        Assert.Equal("slide2", ((NavigateToSlideAction)actions[0]).TargetSlideId);
        Assert.Equal(42, ((SetVariableAction)actions[1]).Value);
        Assert.Equal("layer2", ((ShowLayerAction)actions[2]).LayerId);
        Assert.Equal("layer3", ((HideLayerAction)actions[3]).LayerId);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesVariables()
    {
        // Arrange
        var project = new Project { Name = "Variables Test" };
        project.AddVariable(new Variable { Id = "var1", Name = "BoolVar", Type = VariableType.Boolean, DefaultValue = true });
        project.AddVariable(new Variable { Id = "var2", Name = "NumberVar", Type = VariableType.Number, DefaultValue = 42 });
        project.AddVariable(new Variable { Id = "var3", Name = "StringVar", Type = VariableType.String, DefaultValue = "test" });

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Equal(3, deserialized.Variables.Count);
        Assert.Equal(VariableType.Boolean, deserialized.Variables[0].Type);
        Assert.True((bool)deserialized.Variables[0].DefaultValue!);
        Assert.Equal(VariableType.Number, deserialized.Variables[1].Type);
        Assert.Equal(42, deserialized.Variables[1].DefaultValue);
        Assert.Equal(VariableType.String, deserialized.Variables[2].Type);
        Assert.Equal("test", deserialized.Variables[2].DefaultValue);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesTimeline()
    {
        // Arrange
        var project = new Project { Name = "Timeline Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var textObject = new TextObject
        {
            Id = "text1",
            Timeline = new Timeline { StartTime = 2.5, Duration = 5.0 }
        };
        layer.Objects.Add(textObject);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var deserializedTimeline = ((TextObject)deserialized.Slides[0].Layers[0].Objects[0]).Timeline;
        Assert.NotNull(deserializedTimeline);
        Assert.Equal(2.5, deserializedTimeline!.StartTime);
        Assert.Equal(5.0, deserializedTimeline.Duration);
    }

    [Fact]
    public void Serialize_Deserialize_PreservesComplexProject()
    {
        // Arrange
        var project = new Project
        {
            Name = "Complex Project",
            Version = "1.5.0",
            Author = "John Doe"
        };

        // Add variables
        project.AddVariable(new Variable { Id = "var1", Name = "Score", Type = VariableType.Number, DefaultValue = 0 });
        project.AddVariable(new Variable { Id = "var2", Name = "Completed", Type = VariableType.Boolean, DefaultValue = false });

        // Add slides with layers and objects
        var slide1 = new Slide { Id = "slide1", Title = "Introduction" };
        var layer1 = new Layer { Id = "layer1", Name = "Base" };
        layer1.Objects.Add(new TextObject { Id = "text1", Text = "Welcome" });
        layer1.Objects.Add(new ButtonObject
        {
            Id = "btn1",
            Label = "Next",
            Triggers = new List<Trigger>
            {
                new Trigger
                {
                    Id = "t1",
                    Type = TriggerType.OnClick,
                    Actions = new List<Authoring.Core.Models.Action>
                    {
                        new NavigateToSlideAction { TargetSlideId = "slide2" },
                        new SetVariableAction { VariableId = "var2", Value = true }
                    }
                }
            }
        });
        slide1.Layers.Add(layer1);

        var slide2 = new Slide { Id = "slide2", Title = "Content" };
        var layer2 = new Layer { Id = "layer2", Name = "Base" };
        layer2.Objects.Add(new ImageObject { Id = "img1", SourcePath = "/content.png" });
        slide2.Layers.Add(layer2);

        project.AddSlide(slide1);
        project.AddSlide(slide2);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        Assert.Equal("Complex Project", deserialized.Name);
        Assert.Equal(2, deserialized.Variables.Count);
        Assert.Equal(2, deserialized.Slides.Count);
        Assert.Equal(2, deserialized.Slides[0].Layers[0].Objects.Count);
        Assert.Single(deserialized.Slides[0].Layers[0].Objects[1].Triggers);
        Assert.Equal(2, deserialized.Slides[0].Layers[0].Objects[1].Triggers[0].Actions.Count);
    }

    [Fact]
    public void Serialize_OutputsCamelCaseProperties()
    {
        // Arrange
        var project = new Project { Name = "Test" };

        // Act
        var json = ProjectJsonSerializer.Serialize(project);

        // Assert
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"version\"", json);
        Assert.DoesNotContain("\"Name\"", json);
        Assert.DoesNotContain("\"Version\"", json);
    }

    [Fact]
    public void Deserialize_ThrowsException_WhenJsonIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ProjectJsonSerializer.Deserialize(null!));
    }

    [Fact]
    public void Serialize_ThrowsException_WhenProjectIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ProjectJsonSerializer.Serialize(null!));
    }

    [Fact]
    public void Deserialize_ThrowsException_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Assert.ThrowsAny<System.Text.Json.JsonException>(() => ProjectJsonSerializer.Deserialize(invalidJson));
    }
}
