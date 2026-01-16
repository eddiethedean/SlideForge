using Authoring.Core.Models;

namespace Authoring.Core.Tests.Models;

public class ProjectTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var project = new Project();

        // Assert
        Assert.NotEmpty(project.Id);
        Assert.NotNull(project.Variables);
        Assert.NotNull(project.Slides);
        Assert.Empty(project.Variables);
        Assert.Empty(project.Slides);
        Assert.NotEqual(default(DateTime), project.CreatedAt);
        Assert.NotEqual(default(DateTime), project.ModifiedAt);
        Assert.Equal("1.0.0", project.Version);
    }

    [Fact]
    public void Constructor_GeneratesUniqueIds()
    {
        // Act
        var project1 = new Project();
        var project2 = new Project();

        // Assert
        Assert.NotEqual(project1.Id, project2.Id);
    }

    [Fact]
    public void AddSlide_AddsSlideToCollection()
    {
        // Arrange
        var project = new Project();
        var slide = new Slide { Id = "slide1", Title = "Test Slide" };

        // Act
        project.AddSlide(slide);

        // Assert
        Assert.Single(project.Slides);
        Assert.Equal(slide, project.Slides[0]);
    }

    [Fact]
    public void AddSlide_UpdatesModifiedAt()
    {
        // Arrange
        var project = new Project();
        var initialModifiedAt = project.ModifiedAt;
        var slide = new Slide { Id = "slide1", Title = "Test Slide" };

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Act
        project.AddSlide(slide);

        // Assert
        Assert.True(project.ModifiedAt >= initialModifiedAt);
    }

    [Fact]
    public void AddSlide_ThrowsArgumentNullException_WhenSlideIsNull()
    {
        // Arrange
        var project = new Project();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => project.AddSlide(null!));
    }

    [Fact]
    public void AddVariable_AddsVariableToCollection()
    {
        // Arrange
        var project = new Project();
        var variable = new Variable { Id = "var1", Name = "TestVar", Type = VariableType.Boolean };

        // Act
        project.AddVariable(variable);

        // Assert
        Assert.Single(project.Variables);
        Assert.Equal(variable, project.Variables[0]);
    }

    [Fact]
    public void AddVariable_UpdatesModifiedAt()
    {
        // Arrange
        var project = new Project();
        var initialModifiedAt = project.ModifiedAt;
        var variable = new Variable { Id = "var1", Name = "TestVar", Type = VariableType.Boolean };

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Act
        project.AddVariable(variable);

        // Assert
        Assert.True(project.ModifiedAt >= initialModifiedAt);
    }

    [Fact]
    public void AddVariable_ThrowsArgumentNullException_WhenVariableIsNull()
    {
        // Arrange
        var project = new Project();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => project.AddVariable(null!));
    }

    [Fact]
    public void GetSlideById_ReturnsSlide_WhenExists()
    {
        // Arrange
        var project = new Project();
        var slide = new Slide { Id = "slide1", Title = "Test Slide" };
        project.AddSlide(slide);

        // Act
        var result = project.GetSlideById("slide1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slide, result);
    }

    [Fact]
    public void GetSlideById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var project = new Project();

        // Act
        var result = project.GetSlideById("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetVariableById_ReturnsVariable_WhenExists()
    {
        // Arrange
        var project = new Project();
        var variable = new Variable { Id = "var1", Name = "TestVar", Type = VariableType.Boolean };
        project.AddVariable(variable);

        // Act
        var result = project.GetVariableById("var1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(variable, result);
    }

    [Fact]
    public void GetVariableById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var project = new Project();

        // Act
        var result = project.GetVariableById("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var project = new Project();
        var testId = Guid.NewGuid().ToString();
        var testName = "Test Project";
        var testVersion = "2.0.0";
        var testAuthor = "John Doe";
        var testCreatedAt = DateTime.UtcNow.AddDays(-1);
        var testModifiedAt = DateTime.UtcNow;

        // Act
        project.Id = testId;
        project.Name = testName;
        project.Version = testVersion;
        project.Author = testAuthor;
        project.CreatedAt = testCreatedAt;
        project.ModifiedAt = testModifiedAt;

        // Assert
        Assert.Equal(testId, project.Id);
        Assert.Equal(testName, project.Name);
        Assert.Equal(testVersion, project.Version);
        Assert.Equal(testAuthor, project.Author);
        Assert.Equal(testCreatedAt, project.CreatedAt);
        Assert.Equal(testModifiedAt, project.ModifiedAt);
    }
}
