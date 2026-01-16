using Authoring.Core.Models;
using Authoring.Core.Validation;

namespace Authoring.Core.Tests.Validation;

public class ProjectValidatorTests
{
    [Fact]
    public void ValidateProject_ValidProject_ReturnsNoErrors()
    {
        // Arrange
        var project = new Project { Name = "Valid Project" };
        project.AddSlide(new Slide { Id = "slide1", Title = "Test Slide" });
        project.AddVariable(new Variable { Id = "var1", Name = "TestVar", Type = VariableType.Boolean });

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateProject_NullProject_ReturnsError()
    {
        // Act
        var errors = ProjectValidator.ValidateProject(null!);

        // Assert
        Assert.Single(errors);
        Assert.Contains("cannot be null", errors[0]);
    }

    [Fact]
    public void ValidateProject_EmptyProjectName_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "" };

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Project name cannot be empty"));
    }

    [Fact]
    public void ValidateProject_WhitespaceProjectName_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "   " };

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Project name cannot be empty"));
    }

    [Fact]
    public void ValidateProject_DuplicateSlideIds_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        project.AddSlide(new Slide { Id = "slide1", Title = "Slide 1" });
        project.AddSlide(new Slide { Id = "slide1", Title = "Slide 2" }); // Duplicate ID

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Duplicate slide ID") && e.Contains("slide1"));
    }

    [Fact]
    public void ValidateProject_DuplicateVariableIds_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        project.AddVariable(new Variable { Id = "var1", Name = "Var1", Type = VariableType.Boolean });
        project.AddVariable(new Variable { Id = "var1", Name = "Var2", Type = VariableType.Number }); // Duplicate ID

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Duplicate variable ID") && e.Contains("var1"));
    }

    [Fact]
    public void ValidateProject_EmptySlideTitle_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        project.AddSlide(new Slide { Id = "slide1", Title = "" });

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("slide1") && e.Contains("empty title"));
    }

    [Fact]
    public void ValidateProject_InvalidSlideWidth_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        project.AddSlide(new Slide { Id = "slide1", Title = "Test", Width = -100 });

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("slide1") && e.Contains("invalid width"));
    }

    [Fact]
    public void ValidateProject_InvalidSlideHeight_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        project.AddSlide(new Slide { Id = "slide1", Title = "Test", Height = 0 });

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("slide1") && e.Contains("invalid height"));
    }

    [Fact]
    public void ValidateProject_DuplicateLayerIds_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        slide.Layers.Add(new Layer { Id = "layer1", Name = "Layer 1" });
        slide.Layers.Add(new Layer { Id = "layer1", Name = "Layer 2" }); // Duplicate ID
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Duplicate layer ID") && e.Contains("layer1"));
    }

    [Fact]
    public void ValidateProject_DuplicateObjectIds_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        layer.Objects.Add(new TextObject { Id = "obj1" });
        layer.Objects.Add(new ImageObject { Id = "obj1" }); // Duplicate ID
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Duplicate object ID") && e.Contains("obj1"));
    }

    [Fact]
    public void ValidateProject_TriggerWithNoActions_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger { Id = "trigger1", Type = TriggerType.OnClick, Actions = new List<Authoring.Core.Models.Action>() });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("trigger1") && e.Contains("no actions"));
    }

    [Fact]
    public void ValidateProject_NonExistentVariableReference_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new SetVariableAction { VariableId = "nonexistent", Value = true }
            }
        });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("SetVariableAction") && e.Contains("non-existent variable") && e.Contains("nonexistent"));
    }

    [Fact]
    public void ValidateProject_NonExistentSlideReference_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "nonexistent" }
            }
        });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("NavigateToSlideAction") && e.Contains("non-existent slide") && e.Contains("nonexistent"));
    }

    [Fact]
    public void ValidateProject_NonExistentLayerReference_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new ShowLayerAction { LayerId = "nonexistent" }
            }
        });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Layer action") && e.Contains("non-existent layer") && e.Contains("nonexistent"));
    }

    [Fact]
    public void ValidateProject_NonExistentObjectReference_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            ObjectId = "nonexistent", // References non-existent object
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide1" }
            }
        });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("trigger1") && e.Contains("non-existent object") && e.Contains("nonexistent"));
    }

    [Fact]
    public void ValidateProject_ValidComplexProject_ReturnsNoErrors()
    {
        // Arrange
        var project = new Project { Name = "Complex Project" };
        
        // Add variables
        project.AddVariable(new Variable { Id = "var1", Name = "Score", Type = VariableType.Number });
        project.AddVariable(new Variable { Id = "var2", Name = "Completed", Type = VariableType.Boolean });

        // Add slides
        var slide1 = new Slide { Id = "slide1", Title = "Slide 1" };
        var slide2 = new Slide { Id = "slide2", Title = "Slide 2" };

        // Add layers
        var layer1 = new Layer { Id = "layer1", Name = "Base" };
        var layer2 = new Layer { Id = "layer2", Name = "Overlay" };

        // Add objects with valid references
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" },
                new SetVariableAction { VariableId = "var2", Value = true },
                new ShowLayerAction { LayerId = "layer2" }
            }
        });
        layer1.Objects.Add(button);
        layer1.Objects.Add(new TextObject { Id = "text1" });
        
        slide1.Layers.Add(layer1);
        slide1.Layers.Add(layer2);
        slide2.Layers.Add(new Layer { Id = "layer3", Name = "Base" });

        project.AddSlide(slide1);
        project.AddSlide(slide2);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateProject_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var project = new Project { Name = "" }; // Empty name
        project.AddSlide(new Slide { Id = "slide1", Title = "" }); // Empty title
        project.AddSlide(new Slide { Id = "slide1", Title = "Test", Width = -100 }); // Duplicate ID and invalid width

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.True(errors.Count >= 3); // At least 3 errors
        Assert.Contains(errors, e => e.Contains("Project name cannot be empty"));
        Assert.Contains(errors, e => e.Contains("empty title"));
        Assert.Contains(errors, e => e.Contains("Duplicate slide ID"));
    }

    [Fact]
    public void ValidateProject_HideLayerAction_NonExistentLayer_ReturnsError()
    {
        // Arrange
        var project = new Project { Name = "Test" };
        var slide = new Slide { Id = "slide1", Title = "Test" };
        var layer = new Layer { Id = "layer1", Name = "Base" };
        var button = new ButtonObject { Id = "btn1" };
        button.Triggers.Add(new Trigger
        {
            Id = "trigger1",
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new HideLayerAction { LayerId = "nonexistent" }
            }
        });
        layer.Objects.Add(button);
        slide.Layers.Add(layer);
        project.AddSlide(slide);

        // Act
        var errors = ProjectValidator.ValidateProject(project);

        // Assert
        Assert.Contains(errors, e => e.Contains("Layer action") && e.Contains("non-existent layer") && e.Contains("nonexistent"));
    }
}
