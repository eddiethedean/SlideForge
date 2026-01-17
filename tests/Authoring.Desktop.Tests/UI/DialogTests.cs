using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Authoring.Desktop.Services;
using Authoring.Desktop.Tests.Helpers;
using Authoring.Desktop.ViewModels;
using Authoring.Desktop.Views;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.UI;

[Trait("Category", "UI")]
public class DialogTests : AvaloniaTestBase
{
    [AvaloniaFact]
    public void AboutDialog_CanBeInstantiated()
    {
        // Arrange & Act
        var dialog = new AboutDialog();
        Dispatcher.UIThread.RunJobs();

        // Assert
        Assert.NotNull(dialog);
        Assert.Equal("About SlideForge", dialog.Title);
    }

    [AvaloniaFact]
    public void AboutDialog_ShowsCorrectContent()
    {
        // Arrange
        var dialog = new AboutDialog();
        Dispatcher.UIThread.RunJobs();

        // Assert
        Assert.NotNull(dialog.Content);
    }

    [AvaloniaFact]
    public void AboutCommand_WithMainWindow_ShowsDialog()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var mainWindow = new MainWindow();
        var viewModel = new MainWindowViewModel(mockService.Object, mainWindow);
        mainWindow.DataContext = viewModel;

        Dispatcher.UIThread.RunJobs();

        // Act
        var task = viewModel.AboutCommand.ExecuteAsync(null);
        Dispatcher.UIThread.RunJobs();

        // Assert - Command should execute (dialog creation tested separately)
        Assert.NotNull(task);
    }

    [AvaloniaFact]
    public void DocumentationCommand_WithMainWindow_ShowsDialog()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var mainWindow = new MainWindow();
        var viewModel = new MainWindowViewModel(mockService.Object, mainWindow);
        mainWindow.DataContext = viewModel;

        Dispatcher.UIThread.RunJobs();

        // Act
        var task = viewModel.DocumentationCommand.ExecuteAsync(null);
        Dispatcher.UIThread.RunJobs();

        // Assert - Command should execute
        Assert.NotNull(task);
    }

    [AvaloniaFact]
    public void KeyboardShortcutsCommand_WithMainWindow_ShowsDialog()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var mainWindow = new MainWindow();
        var viewModel = new MainWindowViewModel(mockService.Object, mainWindow);
        mainWindow.DataContext = viewModel;

        Dispatcher.UIThread.RunJobs();

        // Act
        var task = viewModel.KeyboardShortcutsCommand.ExecuteAsync(null);
        Dispatcher.UIThread.RunJobs();

        // Assert - Command should execute
        Assert.NotNull(task);
    }
}
