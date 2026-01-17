using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Validation;

namespace Authoring.Core.Tests.Integration;

[Trait("Category", "Integration")]
public class ProjectIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void CreateModifySerializeDeserialize_CompleteWorkflow_Succeeds()
    {
        // Arrange & Act - Create a complete project
        var project = new Project
        {
            Name = "Complete Workflow Test",
            Version = "1.0.0",
            Author = "Test User"
        };

        // Add variables
        var scoreVar = new Variable { Id = "score", Name = "Score", Type = VariableType.Number, DefaultValue = 0 };
        var completedVar = new Variable { Id = "completed", Name = "Completed", Type = VariableType.Boolean, DefaultValue = false };
        project.AddVariable(scoreVar);
        project.AddVariable(completedVar);

        // Create first slide
        var slide1 = new Slide { Id = "intro", Title = "Introduction", Width = 1920, Height = 1080 };
        var baseLayer1 = new Layer { Id = "base1", Name = "Base Layer" };
        baseLayer1.Objects.Add(new TextObject
        {
            Id = "welcomeText",
            Name = "Welcome Text",
            X = 100,
            Y = 100,
            Width = 800,
            Height = 200,
            Text = "Welcome to the Course",
            FontSize = 24.0,
            Color = "#000000"
        });
        var nextButton = new ButtonObject
        {
            Id = "nextBtn",
            Name = "Next Button",
            X = 800,
            Y = 500,
            Width = 200,
            Height = 50,
            Label = "Next"
        };
        nextButton.Triggers.Add(new Trigger
        {
            Id = "nextTrigger",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "content" },
                new SetVariableAction { VariableId = "completed", Value = true }
            }
        });
        baseLayer1.Objects.Add(nextButton);
        slide1.Layers.Add(baseLayer1);
        project.AddSlide(slide1);

        // Create second slide
        var slide2 = new Slide { Id = "content", Title = "Content", Width = 1920, Height = 1080 };
        var baseLayer2 = new Layer { Id = "base2", Name = "Base Layer" };
        baseLayer2.Objects.Add(new ImageObject
        {
            Id = "contentImage",
            Name = "Content Image",
            X = 200,
            Y = 200,
            Width = 600,
            Height = 400,
            SourcePath = "/images/content.png",
            MaintainAspectRatio = true
        });
        var textWithTimeline = new TextObject
        {
            Id = "animatedText",
            Name = "Animated Text",
            X = 900,
            Y = 300,
            Width = 400,
            Height = 100,
            Text = "This text appears after 2 seconds",
            Timeline = new Timeline { StartTime = 2.0, Duration = 5.0 }
        };
        baseLayer2.Objects.Add(textWithTimeline);
        slide2.Layers.Add(baseLayer2);
        project.AddSlide(slide2);

        // Validate project
        var validationErrors = ProjectValidator.ValidateProject(project);
        Assert.Empty(validationErrors);

        // Serialize
        var json = ProjectJsonSerializer.Serialize(project);

        // Assert JSON contains expected data
        Assert.Contains("Complete Workflow Test", json);
        Assert.Contains("\"score\"", json);
        Assert.Contains("\"intro\"", json);
        Assert.Contains("\"content\"", json);

        // Deserialize
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert deserialized project is equivalent
        Assert.Equal(project.Name, deserialized.Name);
        Assert.Equal(2, deserialized.Variables.Count);
        Assert.Equal(2, deserialized.Slides.Count);

        // Verify first slide structure
        var deserializedSlide1 = deserialized.GetSlideById("intro");
        Assert.NotNull(deserializedSlide1);
        Assert.Single(deserializedSlide1!.Layers);
        Assert.Equal(2, deserializedSlide1.Layers[0].Objects.Count);
        
        var deserializedButton = deserializedSlide1.Layers[0].Objects[1] as ButtonObject;
        Assert.NotNull(deserializedButton);
        Assert.Equal("Next", deserializedButton!.Label);
        Assert.Single(deserializedButton.Triggers);
        Assert.Equal(2, deserializedButton.Triggers[0].Actions.Count);

        // Verify second slide structure
        var deserializedSlide2 = deserialized.GetSlideById("content");
        Assert.NotNull(deserializedSlide2);
        var deserializedImage = deserializedSlide2!.Layers[0].Objects[0] as ImageObject;
        Assert.NotNull(deserializedImage);
        Assert.Equal("/images/content.png", deserializedImage!.SourcePath);

        var deserializedAnimatedText = deserializedSlide2.Layers[0].Objects[1] as TextObject;
        Assert.NotNull(deserializedAnimatedText);
        Assert.NotNull(deserializedAnimatedText!.Timeline);
        Assert.Equal(2.0, deserializedAnimatedText.Timeline!.StartTime);
        Assert.Equal(5.0, deserializedAnimatedText.Timeline.Duration);

        // Re-validate deserialized project
        var revalidationErrors = ProjectValidator.ValidateProject(deserialized);
        Assert.Empty(revalidationErrors);
    }

    [Fact]
    public void ComplexTriggerChain_SerializeDeserialize_PreservesCorrectly()
    {
        // Arrange - Create a project with complex trigger chains
        var project = new Project { Name = "Trigger Chain Test" };
        
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        
        var button = new ButtonObject { Id = "btn1", Label = "Action Button" };
        
        // Multiple triggers with different types
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" },
                new SetVariableAction { VariableId = "var1", Value = 10 },
                new ShowLayerAction { LayerId = "layer2" },
                new HideLayerAction { LayerId = "layer3" }
            }
        });
        
        button.Triggers.Add(new Trigger
        {
            Id = "trigger2",
            Type = TriggerType.OnTimelineStart,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new SetVariableAction { VariableId = "var2", Value = "started" }
            }
        });
        
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var json = ProjectJsonSerializer.Serialize(project);
        var deserialized = ProjectJsonSerializer.Deserialize(json);

        // Assert
        var deserializedButton = deserialized.Slides[0].Layers[0].Objects[0] as ButtonObject;
        Assert.NotNull(deserializedButton);
        Assert.Equal(2, deserializedButton!.Triggers.Count);
        
        var clickTrigger = deserializedButton.Triggers.FirstOrDefault(t => t.Type == TriggerType.OnClick);
        Assert.NotNull(clickTrigger);
        Assert.Equal(4, clickTrigger!.Actions.Count);
        
        var timelineTrigger = deserializedButton.Triggers.FirstOrDefault(t => t.Type == TriggerType.OnTimelineStart);
        Assert.NotNull(timelineTrigger);
        Assert.Single(timelineTrigger!.Actions);
    }

    [Fact]
    public void ProjectWithAllObjectTypes_SerializeDeserialize_AllTypesPreserved()
    {
        // Arrange
        var project = new Project { Name = "All Types Test" };
        var slide = new Slide { Id = "slide1", Title = "All Objects" };
        var layer = new Layer { Id = "layer1", Name = "Base" };

        // Add all object types
        layer.Objects.Add(new TextObject { Id = "text1", Text = "Text Content" });
        layer.Objects.Add(new ImageObject { Id = "img1", SourcePath = "/image.jpg" });
        layer.Objects.Add(new ButtonObject { Id = "btn1", Label = "Button Label" });

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
    }
}
