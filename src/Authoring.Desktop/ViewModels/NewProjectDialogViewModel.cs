using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class NewProjectDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string? _author;

    public bool IsValid => !string.IsNullOrWhiteSpace(ProjectName);

    [RelayCommand]
    private void Ok()
    {
        // Dialog result handled by view
    }

    [RelayCommand]
    private void Cancel()
    {
        // Dialog result handled by view
    }
}
