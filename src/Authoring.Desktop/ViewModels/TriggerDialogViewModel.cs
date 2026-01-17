using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Authoring.Core.Models;
using Authoring.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class TriggerDialogViewModel : ViewModelBase
{
    private readonly SlideObject? _selectedObject;
    private readonly Project? _project;
    private readonly Slide? _currentSlide;
    private TriggerType _triggerType;

    public TriggerDialogViewModel(SlideObject? selectedObject = null, Project? project = null, Slide? currentSlide = null, Trigger? existingTrigger = null)
    {
        _selectedObject = selectedObject;
        _project = project;
        _currentSlide = currentSlide;

        AvailableTriggerTypes = new ObservableCollection<TriggerType>();
        Actions = new ObservableCollection<Authoring.Core.Models.Action>();

        // Determine available trigger types based on object type
        if (selectedObject != null)
        {
            // OnClick available for ButtonObject (and potentially others in future)
            if (selectedObject is ButtonObject)
            {
                AvailableTriggerTypes.Add(TriggerType.OnClick);
            }
            // OnTimelineStart available for all objects
            AvailableTriggerTypes.Add(TriggerType.OnTimelineStart);
        }

        // Load existing trigger data if editing
        if (existingTrigger != null)
        {
            _triggerType = existingTrigger.Type;
            foreach (var action in existingTrigger.Actions)
            {
                Actions.Add(action);
            }
        }
        else if (AvailableTriggerTypes.Count > 0)
        {
            _triggerType = AvailableTriggerTypes[0];
        }
    }

    public ObservableCollection<TriggerType> AvailableTriggerTypes { get; }

    public TriggerType TriggerType
    {
        get => _triggerType;
        set
        {
            if (SetProperty(ref _triggerType, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public ObservableCollection<Authoring.Core.Models.Action> Actions { get; }

    public bool IsValid => Actions.Count > 0;

    // Commands will be set by the dialog to handle closing
    public IRelayCommand? OkCommand { get; set; }
    public IRelayCommand? CancelCommand { get; set; }

    [RelayCommand]
    private async Task AddActionAsync()
    {
        var dialog = new ActionDialog
        {
            DataContext = new ActionDialogViewModel(_project, _currentSlide)
        };

        if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.OfType<TriggerDialog>().LastOrDefault();
            if (owner != null)
            {
                var result = await dialog.ShowDialog<ActionDialogViewModel?>(owner);
                if (result != null && result.IsValid)
                {
                    var action = result.BuildAction();
                    if (action != null)
                    {
                        Actions.Add(action);
                        OnPropertyChanged(nameof(IsValid));
                    }
                }
            }
        }
    }

    [RelayCommand]
    public async Task EditActionAsync(object? parameter)
    {
        if (parameter is not Authoring.Core.Models.Action action) return;

        var dialog = new ActionDialog
        {
            DataContext = new ActionDialogViewModel(_project, _currentSlide, action)
        };

        if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.OfType<TriggerDialog>().LastOrDefault();
            if (owner != null)
            {
                var result = await dialog.ShowDialog<ActionDialogViewModel?>(owner);
                if (result != null && result.IsValid)
                {
                    var index = Actions.IndexOf(action);
                    if (index >= 0)
                    {
                        var newAction = result.BuildAction();
                        if (newAction != null)
                        {
                            Actions.RemoveAt(index);
                            Actions.Insert(index, newAction);
                        }
                    }
                }
            }
        }
    }

    [RelayCommand]
    public void DeleteAction(object? parameter)
    {
        if (parameter is Authoring.Core.Models.Action action)
        {
            Actions.Remove(action);
            OnPropertyChanged(nameof(IsValid));
        }
    }
}
