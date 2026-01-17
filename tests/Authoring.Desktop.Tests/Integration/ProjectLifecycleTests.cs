using System.IO;
using System.Threading.Tasks;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Desktop.Services;
using Xunit;

namespace Authoring.Desktop.Tests.Integration;

[Trait("Category", "Integration")]
public class ProjectLifecycleTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly ProjectService _service;

    public ProjectLifecycleTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _service = new ProjectService();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Fact]
    public async Task CreateEditSaveLoad_CompleteLifecycle_PreservesAllChanges()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "lifecycle.json");

        // Act - Create project
        var project = _service.CreateNewProject("Lifecycle Test", "Test Author");
        
        // Act - Edit: Add slides and objects
        var slide1 = SlideBuilder.Create()
            .WithTitle("First Slide")
            .WithObject(o => o
                .AtPosition(100, 100)
                .BuildTextObject("First Text"))
            .Build();

        var slide2 = SlideBuilder.Create()
            .WithTitle("Second Slide")
            .WithObject(o => o
                .AtPosition(200, 200)
                .BuildImageObject("image.png"))
            .Build();

        project.AddSlide(slide1);
        project.AddSlide(slide2);

        // Act - Save
        await _service.SaveProjectAsync(project, filePath);

        // Act - Load
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.NotNull(loadedProject);
        Assert.Equal(project.Name, loadedProject!.Name);
        Assert.Equal(project.Author, loadedProject.Author);
        Assert.Equal(3, loadedProject.Slides.Count); // 1 default + 2 added
        Assert.Contains(loadedProject.Slides, s => s.Title == "First Slide");
        Assert.Contains(loadedProject.Slides, s => s.Title == "Second Slide");
    }

    [Fact]
    public async Task CreateModifySaveLoadModifySave_IterativeEditing_PreservesChanges()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "iterative.json");

        // Act - Create and save
        var project1 = _service.CreateNewProject("Iterative Test");
        await _service.SaveProjectAsync(project1, filePath);

        // Act - Load, modify, save
        var project2 = await _service.OpenProjectAsync(filePath);
        Assert.NotNull(project2);
        project2!.Name = "Modified Name";
        project2.Slides[0].Title = "Modified Slide";
        await _service.SaveProjectAsync(project2, filePath);

        // Act - Load again, verify modifications
        var project3 = await _service.OpenProjectAsync(filePath);
        Assert.NotNull(project3);
        Assert.Equal("Modified Name", project3!.Name);
        Assert.Equal("Modified Slide", project3.Slides[0].Title);
    }

    [Fact]
    public async Task CreateAddObjectsEditPropertiesSaveLoad_PropertyChanges_Preserved()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "properties.json");

        // Act - Create project with objects
        var project = _service.CreateNewProject("Properties Test");
        var slide = project.Slides[0];
        
        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Text",
            X = 100,
            Y = 200,
            Width = 300,
            Height = 50,
            Text = "Initial Text",
            FontSize = 16,
            Color = "#000000"
        };
        slide.Layers[0].Objects.Add(textObject);

        // Act - Save
        await _service.SaveProjectAsync(project, filePath);

        // Act - Load and modify properties
        var loadedProject = await _service.OpenProjectAsync(filePath);
        var loadedTextObject = loadedProject!.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        
        loadedTextObject.X = 200;
        loadedTextObject.Y = 300;
        loadedTextObject.Text = "Modified Text";
        loadedTextObject.FontSize = 24;
        loadedTextObject.Color = "#FF0000";

        // Act - Save again
        await _service.SaveProjectAsync(loadedProject, filePath);

        // Act - Load and verify
        var finalProject = await _service.OpenProjectAsync(filePath);
        var finalTextObject = finalProject!.Slides[0].Layers[0].Objects.OfType<TextObject>().First();

        // Assert
        Assert.Equal(200, finalTextObject.X);
        Assert.Equal(300, finalTextObject.Y);
        Assert.Equal("Modified Text", finalTextObject.Text);
        Assert.Equal(24, finalTextObject.FontSize);
        Assert.Equal("#FF0000", finalTextObject.Color);
    }

    [Fact]
    public async Task CreateAddTriggersSaveLoad_TriggersAndActions_Preserved()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "triggers.json");

        // Act - Create project with triggers
        var project = _service.CreateNewProject("Triggers Test");
        var slide1 = SlideBuilder.Create()
            .WithId("slide1")
            .WithTitle("Slide 1")
            .Build();
        var slide2 = SlideBuilder.Create()
            .WithId("slide2")
            .WithTitle("Slide 2")
            .Build();

        project.AddSlide(slide1);
        project.AddSlide(slide2);

        var button = new ButtonObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Button",
            X = 100,
            Y = 100,
            Width = 150,
            Height = 40,
            Label = "Navigate"
        };

        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new NavigateToSlideAction { TargetSlideId = "slide2" }
            }
        });

        slide1.Layers[0].Objects.Add(button);

        // Act - Save and load
        await _service.SaveProjectAsync(project, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        var loadedButton = loadedProject!.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        Assert.Single(loadedButton.Triggers);
        
        var trigger = loadedButton.Triggers[0];
        Assert.Equal(TriggerType.OnClick, trigger.Type);
        Assert.Single(trigger.Actions);
        
        var action = trigger.Actions[0] as NavigateToSlideAction;
        Assert.NotNull(action);
        Assert.Equal("slide2", action!.TargetSlideId);
    }

    [Fact]
    public async Task CreateAddVariablesUseInActionsSaveLoad_VariableReferences_Preserved()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "variables.json");

        // Act - Create project with variables and actions
        var project = _service.CreateNewProject("Variables Test");
        project.AddVariable(new Variable
        {
            Id = "counter",
            Name = "Counter",
            Type = VariableType.Number,
            DefaultValue = 0
        });

        var slide = project.Slides[0];
        var button = new ButtonObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Button",
            X = 100,
            Y = 100,
            Width = 150,
            Height = 40,
            Label = "Increment"
        };

        button.Triggers.Add(new Trigger
        {
            Id = Guid.NewGuid().ToString(),
            Type = TriggerType.OnClick,
            Actions = new List<Authoring.Core.Models.Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 10 }
            }
        });

        slide.Layers[0].Objects.Add(button);

        // Act - Save and load
        await _service.SaveProjectAsync(project, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.Single(loadedProject!.Variables);
        Assert.Equal("counter", loadedProject.Variables[0].Id);

        var loadedButton = loadedProject.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var setAction = loadedButton.Triggers[0].Actions.OfType<SetVariableAction>().First();
        Assert.Equal("counter", setAction.VariableId);
        Assert.Equal(10, setAction.Value);
    }

    [Fact]
    public async Task CreateComplexProjectSaveLoad_MultipleOperations_PreservesEverything()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "complex.json");

        // Act - Create complex project using factory
        var project = TestDataFactory.CreateComplexProject();

        // Act - Save and load
        await _service.SaveProjectAsync(project, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert - Verify all components
        Assert.NotNull(loadedProject);
        Assert.Equal(project.Name, loadedProject!.Name);
        Assert.Equal(project.Variables.Count, loadedProject.Variables.Count);
        Assert.Equal(project.Slides.Count, loadedProject.Slides.Count);

        // Verify objects and triggers
        var originalButton = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        var loadedButton = loadedProject.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        
        Assert.Equal(originalButton.Triggers.Count, loadedButton.Triggers.Count);
        Assert.Equal(originalButton.Label, loadedButton.Label);
    }
}
