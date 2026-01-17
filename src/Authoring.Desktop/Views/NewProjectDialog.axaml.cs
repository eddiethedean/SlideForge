using Avalonia.Controls;
using Authoring.Desktop.ViewModels;

namespace Authoring.Desktop.Views;

public partial class NewProjectDialog : Window
{
    public NewProjectDialog()
    {
        InitializeComponent();
    }

    public NewProjectDialogViewModel ViewModel => (NewProjectDialogViewModel)DataContext!;

    public string? ProjectName => ViewModel.IsValid ? ViewModel.ProjectName : null;
    public string? Author => ViewModel.Author;
}
