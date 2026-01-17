using Avalonia.Controls;
using Authoring.Desktop.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.Views;

public partial class VariableDialog : Window
{
    private RelayCommand? _okCommand;

    public VariableDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        
        if (DataContext is VariableDialogViewModel vm)
        {
            // Wire up OK command to close dialog with result
            _okCommand = new RelayCommand(() =>
            {
                if (vm.IsValid)
                {
                    Close(vm);
                }
            }, () => vm.IsValid);

            vm.OkCommand = _okCommand;

            // Cancel button with IsCancel="True" will automatically close with null
            vm.CancelCommand = new RelayCommand(() =>
            {
                Close(null);
            });

            // Update OK command when IsValid changes
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Name) || e.PropertyName == nameof(vm.Type))
                {
                    _okCommand?.NotifyCanExecuteChanged();
                }
            };
        }
    }

    public VariableDialogViewModel? ViewModel => DataContext as VariableDialogViewModel;
}
