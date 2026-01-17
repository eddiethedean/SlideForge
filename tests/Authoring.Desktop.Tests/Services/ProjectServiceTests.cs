using System.IO;
using System.Text;
using System.Threading.Tasks;
using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Desktop.Services;
using Xunit;

namespace Authoring.Desktop.Tests.Services;

public class ProjectServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly ProjectService _service;

    public ProjectServiceTests()
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
    public void CreateNewProject_WithName_ReturnsProjectWithDefaultSlide()
    {
        // Act
        var project = _service.CreateNewProject("Test Project");

        // Assert
        Assert.NotNull(project);
        Assert.Equal("Test Project", project.Name);
        Assert.NotEmpty(project.Id);
        Assert.Single(project.Slides);
        Assert.Equal("Slide 1", project.Slides[0].Title);
        Assert.Equal(1920, project.Slides[0].Width);
        Assert.Equal(1080, project.Slides[0].Height);
        Assert.Single(project.Slides[0].Layers);
        Assert.Equal("Base Layer", project.Slides[0].Layers[0].Name);
    }

    [Fact]
    public void CreateNewProject_WithNameAndAuthor_ReturnsProjectWithAuthor()
    {
        // Act
        var project = _service.CreateNewProject("Test Project", "Test Author");

        // Assert
        Assert.NotNull(project);
        Assert.Equal("Test Project", project.Name);
        Assert.Equal("Test Author", project.Author);
    }

    [Fact]
    public void CreateNewProject_WithNullAuthor_ReturnsProjectWithNullAuthor()
    {
        // Act
        var project = _service.CreateNewProject("Test Project", null);

        // Assert
        Assert.NotNull(project);
        Assert.Null(project.Author);
    }

    [Fact]
    public async Task SaveProjectAsync_ValidProject_SavesToFile()
    {
        // Arrange
        var project = _service.CreateNewProject("Test Project");
        var filePath = Path.Combine(_testDirectory, "test.json");

        // Act
        await _service.SaveProjectAsync(project, filePath);

        // Assert
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.NotEmpty(content);
        Assert.Contains("Test Project", content);
    }

    [Fact]
    public async Task SaveProjectAsync_UpdatesModifiedAt()
    {
        // Arrange
        var project = _service.CreateNewProject("Test Project");
        var originalModifiedAt = project.ModifiedAt;
        var filePath = Path.Combine(_testDirectory, "test.json");

        // Act
        await Task.Delay(10); // Small delay to ensure timestamp difference
        await _service.SaveProjectAsync(project, filePath);

        // Assert
        Assert.True(project.ModifiedAt > originalModifiedAt);
    }

    [Fact]
    public async Task SaveProjectAsync_CreatesValidJson()
    {
        // Arrange
        var project = _service.CreateNewProject("Test Project");
        var filePath = Path.Combine(_testDirectory, "test.json");

        // Act
        await _service.SaveProjectAsync(project, filePath);

        // Assert
        var json = await File.ReadAllTextAsync(filePath);
        var deserialized = ProjectJsonSerializer.Deserialize(json);
        Assert.NotNull(deserialized);
        Assert.Equal(project.Name, deserialized.Name);
        Assert.Equal(project.Id, deserialized.Id);
    }

    [Fact]
    public async Task OpenProjectAsync_ValidFile_ReturnsProject()
    {
        // Arrange
        var project = _service.CreateNewProject("Test Project");
        var filePath = Path.Combine(_testDirectory, "test.json");
        await _service.SaveProjectAsync(project, filePath);

        // Act
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.NotNull(loadedProject);
        Assert.Equal(project.Name, loadedProject!.Name);
        Assert.Equal(project.Id, loadedProject.Id);
        Assert.Equal(project.Slides.Count, loadedProject.Slides.Count);
    }

    [Fact]
    public async Task OpenProjectAsync_NonExistentFile_ReturnsNull()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "nonexistent.json");

        // Act
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.Null(loadedProject);
    }

    [Fact]
    public async Task OpenProjectAsync_InvalidJson_ReturnsNull()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "invalid.json");
        await File.WriteAllTextAsync(filePath, "{ invalid json }");

        // Act
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.Null(loadedProject);
    }

    [Fact]
    public async Task SaveAndOpenProjectAsync_RoundTrip_PreservesAllData()
    {
        // Arrange
        var originalProject = _service.CreateNewProject("Test Project", "Test Author");
        originalProject.AddVariable(new Variable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestVar",
            Type = VariableType.Number,
            DefaultValue = 42
        });

        var slide = originalProject.Slides[0];
        var textObject = new TextObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Text",
            X = 100,
            Y = 200,
            Width = 300,
            Height = 50,
            Text = "Hello World"
        };
        slide.Layers[0].Objects.Add(textObject);

        var filePath = Path.Combine(_testDirectory, "roundtrip.json");

        // Act
        await _service.SaveProjectAsync(originalProject, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.NotNull(loadedProject);
        Assert.Equal(originalProject.Name, loadedProject!.Name);
        Assert.Equal(originalProject.Author, loadedProject.Author);
        Assert.Equal(originalProject.Variables.Count, loadedProject.Variables.Count);
        Assert.Equal(originalProject.Slides.Count, loadedProject.Slides.Count);
        Assert.Single(loadedProject.Slides[0].Layers[0].Objects);
        
        var loadedTextObject = loadedProject.Slides[0].Layers[0].Objects[0] as TextObject;
        Assert.NotNull(loadedTextObject);
        Assert.Equal(textObject.Text, loadedTextObject!.Text);
        Assert.Equal(textObject.X, loadedTextObject.X);
        Assert.Equal(textObject.Y, loadedTextObject.Y);
    }

    [Fact]
    public async Task OpenProjectAsync_FileWithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var project = _service.CreateNewProject("Test \"Project\" with 'quotes' & symbols");
        var filePath = Path.Combine(_testDirectory, "special.json");

        // Act
        await _service.SaveProjectAsync(project, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.NotNull(loadedProject);
        Assert.Equal(project.Name, loadedProject!.Name);
    }

    [Fact]
    public async Task SaveProjectAsync_WithUnicodeCharacters_PreservesCorrectly()
    {
        // Arrange
        var project = _service.CreateNewProject("测试项目");
        project.Author = "Автор";
        var filePath = Path.Combine(_testDirectory, "unicode.json");

        // Act
        await _service.SaveProjectAsync(project, filePath);
        var loadedProject = await _service.OpenProjectAsync(filePath);

        // Assert
        Assert.NotNull(loadedProject);
        Assert.Equal("测试项目", loadedProject!.Name);
        Assert.Equal("Автор", loadedProject.Author);
    }
}
