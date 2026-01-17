using Avalonia.Headless;
using Authoring.Desktop.ViewModels;
using Authoring.Desktop.Views;
using Xunit;

namespace Authoring.Desktop.Tests.UI;

[Trait("Category", "UI")]
public class ViewTests
{
    [Fact]
    public void MainWindow_CanBeInstantiated()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        // Act
        var window = new MainWindow();

        // Assert
        Assert.NotNull(window);
    }

    [Fact]
    public void MainWindow_WithViewModel_LoadsCorrectly()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var mockService = new Moq.Mock<Services.IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var window = new MainWindow
        {
            DataContext = viewModel
        };

        // Act & Assert - Should not throw
        Assert.NotNull(window);
        Assert.NotNull(window.DataContext);
    }

    [Fact]
    public void NewProjectDialog_CanBeInstantiated()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        // Act
        var dialog = new Views.NewProjectDialog();

        // Assert
        Assert.NotNull(dialog);
    }

    [Fact]
    public void NewProjectDialog_WithViewModel_LoadsCorrectly()
    {
        // Arrange
        AppBuilder.Configure<Avalonia.Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var dialog = new Views.NewProjectDialog
        {
            DataContext = new NewProjectDialogViewModel()
        };

        // Act & Assert - Should not throw
        Assert.NotNull(dialog);
        Assert.NotNull(dialog.DataContext);
    }
}
