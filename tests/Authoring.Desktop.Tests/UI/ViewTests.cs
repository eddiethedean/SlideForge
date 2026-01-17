using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Authoring.Desktop.Services;
using Authoring.Desktop.Tests.Helpers;
using Authoring.Desktop.ViewModels;
using Authoring.Desktop.Views;
using Xunit;

namespace Authoring.Desktop.Tests.UI;

[Trait("Category", "UI")]
public class ViewTests : AvaloniaTestBase
{
    [AvaloniaFact]
    public void MainWindow_CanBeInstantiated()
    {
        // Arrange & Act & Assert
        var window = new MainWindow();
        Dispatcher.UIThread.RunJobs();
        Assert.NotNull(window);
    }

    [AvaloniaFact]
    public void MainWindow_WithViewModel_LoadsCorrectly()
    {
        // Arrange
        var mockService = new Moq.Mock<Authoring.Desktop.Services.IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        // Act & Assert - Should not throw
        var window = new MainWindow
        {
            DataContext = viewModel
        };
        Dispatcher.UIThread.RunJobs();
        
        Assert.NotNull(window);
        Assert.NotNull(window.DataContext);
    }

    [AvaloniaFact]
    public void NewProjectDialog_CanBeInstantiated()
    {
        // Arrange & Act & Assert
        var dialog = new Views.NewProjectDialog();
        Dispatcher.UIThread.RunJobs();
        Assert.NotNull(dialog);
    }

    [AvaloniaFact]
    public void NewProjectDialog_WithViewModel_LoadsCorrectly()
    {
        // Arrange & Act & Assert
        var dialog = new Views.NewProjectDialog
        {
            DataContext = new NewProjectDialogViewModel()
        };
        Dispatcher.UIThread.RunJobs();
        
        Assert.NotNull(dialog);
        Assert.NotNull(dialog.DataContext);
    }
}
