using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class NewProjectDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string? _author;

    // Commands will be set by the dialog to handle closing
    public IRelayCommand? OkCommand { get; set; }
    public IRelayCommand? CancelCommand { get; set; }

    public bool IsValid => !string.IsNullOrWhiteSpace(ProjectName);
}
