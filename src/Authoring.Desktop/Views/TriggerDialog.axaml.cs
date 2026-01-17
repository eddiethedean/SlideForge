using Avalonia.Controls;
using Avalonia.Interactivity;
using Authoring.Core.Models;
using Authoring.Desktop.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.Views;

public partial class TriggerDialog : Window
{
    private RelayCommand? _okCommand;

    public TriggerDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        
        if (DataContext is TriggerDialogViewModel vm)
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
                if (e.PropertyName == nameof(vm.IsValid))
                {
                    _okCommand?.NotifyCanExecuteChanged();
                }
            };
        }
    }

    public TriggerDialogViewModel? ViewModel => DataContext as TriggerDialogViewModel;

    private async void OnEditActionClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Authoring.Core.Models.Action action && ViewModel != null)
        {
            await ViewModel.EditActionAsync(action);
        }
    }

    private void OnDeleteActionClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Authoring.Core.Models.Action action && ViewModel != null)
        {
            ViewModel.DeleteAction(action);
        }
    }
}
