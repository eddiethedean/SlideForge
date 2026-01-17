using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;

namespace Authoring.Desktop.Tests.Helpers;

/// <summary>
/// Base class for ViewModel tests with common setup.
/// </summary>
public abstract class ViewModelTestBase
{
    protected Mock<IProjectService> MockProjectService { get; }
    protected MainWindowViewModel ViewModel { get; }

    protected ViewModelTestBase()
    {
        MockProjectService = new Mock<IProjectService>();
        ViewModel = new MainWindowViewModel(MockProjectService.Object);
    }
}
