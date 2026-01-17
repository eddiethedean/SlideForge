using Authoring.Desktop.ViewModels;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class NewProjectDialogViewModelTests
{
    [Fact]
    public void Constructor_InitializesEmptyProperties()
    {
        // Act
        var viewModel = new NewProjectDialogViewModel();

        // Assert
        Assert.Empty(viewModel.ProjectName);
        Assert.Null(viewModel.Author);
        Assert.False(viewModel.IsValid);
    }

    [Fact]
    public void IsValid_EmptyProjectName_ReturnsFalse()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();

        // Assert
        Assert.False(viewModel.IsValid);
    }

    [Fact]
    public void IsValid_WhitespaceProjectName_ReturnsFalse()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();
        viewModel.ProjectName = "   ";

        // Assert
        Assert.False(viewModel.IsValid);
    }

    [Fact]
    public void IsValid_ValidProjectName_ReturnsTrue()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();
        viewModel.ProjectName = "Test Project";

        // Assert
        Assert.True(viewModel.IsValid);
    }

    [Fact]
    public void ProjectName_SettingValue_UpdatesIsValid()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();

        // Act
        viewModel.ProjectName = "My Project";

        // Assert
        Assert.True(viewModel.IsValid);
        Assert.Equal("My Project", viewModel.ProjectName);
    }

    [Fact]
    public void Author_SettingValue_StoresCorrectly()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();

        // Act
        viewModel.Author = "John Doe";

        // Assert
        Assert.Equal("John Doe", viewModel.Author);
    }

    [Fact]
    public void Author_CanBeNull()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();
        viewModel.Author = "John Doe";

        // Act
        viewModel.Author = null;

        // Assert
        Assert.Null(viewModel.Author);
    }

    [Fact]
    public void IsValid_WithProjectName_IsIndependentOfAuthor()
    {
        // Arrange
        var viewModel = new NewProjectDialogViewModel();

        // Act
        viewModel.ProjectName = "Test";
        viewModel.Author = null;

        // Assert
        Assert.True(viewModel.IsValid);

        // Act
        viewModel.Author = "Author";

        // Assert
        Assert.True(viewModel.IsValid);
    }
}
