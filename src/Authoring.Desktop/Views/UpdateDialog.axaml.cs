using Avalonia.Controls;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;

namespace Authoring.Desktop.Views;

public partial class UpdateDialog : Window
{
    public UpdateDialog()
    {
        InitializeComponent();
    }

    public UpdateDialog(IUpdateService updateService) : this()
    {
        DataContext = new UpdateDialogViewModel(updateService);
    }
}
