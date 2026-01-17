using Avalonia.Controls;
using Authoring.Desktop.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Authoring.Desktop.Views;

public partial class NewProjectDialog : Window
{
    private RelayCommand? _okCommand;

    public NewProjectDialog()
    {
        InitializeComponent();
        var vm = new NewProjectDialogViewModel();
        DataContext = vm;
        
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
            if (e.PropertyName == nameof(vm.ProjectName))
            {
                _okCommand?.NotifyCanExecuteChanged();
            }
        };
    }

    public NewProjectDialogViewModel ViewModel => (NewProjectDialogViewModel)DataContext!;
}
