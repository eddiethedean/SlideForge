using System;
using System.Collections.ObjectModel;
using System.Linq;
using Authoring.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class ActionDialogViewModel : ViewModelBase
{
    private readonly Project? _project;
    private readonly Slide? _currentSlide;
    private ActionType _selectedActionType;
    private Slide? _selectedSlide;
    private Variable? _selectedVariable;
    private Layer? _selectedLayer;
    private bool _booleanValue;
    private double _numberValue;
    private string _stringValue = string.Empty;

    public ActionDialogViewModel(Project? project = null, Slide? currentSlide = null, Authoring.Core.Models.Action? existingAction = null)
    {
        _project = project;
        _currentSlide = currentSlide;

        ActionTypes = new ObservableCollection<ActionType>
        {
            ActionType.NavigateToSlide,
            ActionType.SetVariable,
            ActionType.ShowLayer,
            ActionType.HideLayer
        };

        AvailableSlides = new ObservableCollection<Slide>();
        AvailableVariables = new ObservableCollection<Variable>();
        AvailableLayers = new ObservableCollection<Layer>();

        // Load available options
        if (_project != null)
        {
            foreach (var slide in _project.Slides)
            {
                AvailableSlides.Add(slide);
            }
            foreach (var variable in _project.Variables)
            {
                AvailableVariables.Add(variable);
            }
        }

        if (_currentSlide != null)
        {
            foreach (var layer in _currentSlide.Layers)
            {
                AvailableLayers.Add(layer);
            }
        }

        // Load existing action data if editing
        if (existingAction != null)
        {
            _selectedActionType = existingAction.Type;

            switch (existingAction)
            {
                case NavigateToSlideAction navAction:
                    _selectedSlide = AvailableSlides.FirstOrDefault(s => s.Id == navAction.TargetSlideId);
                    break;
                case SetVariableAction setVarAction:
                    _selectedVariable = AvailableVariables.FirstOrDefault(v => v.Id == setVarAction.VariableId);
                    if (_selectedVariable != null)
                    {
                        switch (_selectedVariable.Type)
                        {
                            case VariableType.Boolean:
                                _booleanValue = setVarAction.Value is bool b && b;
                                break;
                            case VariableType.Number:
                                _numberValue = setVarAction.Value is double d ? d : (setVarAction.Value is int i ? i : 0);
                                break;
                            case VariableType.String:
                                _stringValue = setVarAction.Value as string ?? string.Empty;
                                break;
                        }
                    }
                    break;
                case ShowLayerAction showAction:
                    _selectedLayer = AvailableLayers.FirstOrDefault(l => l.Id == showAction.LayerId);
                    break;
                case HideLayerAction hideAction:
                    _selectedLayer = AvailableLayers.FirstOrDefault(l => l.Id == hideAction.LayerId);
                    break;
            }
        }
        else
        {
            _selectedActionType = ActionTypes[0];
        }

        // Update visibility properties
        UpdateActionTypeVisibility();
    }

    public ObservableCollection<ActionType> ActionTypes { get; }
    public ObservableCollection<Slide> AvailableSlides { get; }
    public ObservableCollection<Variable> AvailableVariables { get; }
    public ObservableCollection<Layer> AvailableLayers { get; }

    public ActionType SelectedActionType
    {
        get => _selectedActionType;
        set
        {
            if (SetProperty(ref _selectedActionType, value))
            {
                UpdateActionTypeVisibility();
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public Slide? SelectedSlide
    {
        get => _selectedSlide;
        set
        {
            if (SetProperty(ref _selectedSlide, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public Variable? SelectedVariable
    {
        get => _selectedVariable;
        set
        {
            if (SetProperty(ref _selectedVariable, value))
            {
                UpdateVariableTypeVisibility();
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public Layer? SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            if (SetProperty(ref _selectedLayer, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public bool BooleanValue
    {
        get => _booleanValue;
        set
        {
            if (SetProperty(ref _booleanValue, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public double NumberValue
    {
        get => _numberValue;
        set
        {
            if (SetProperty(ref _numberValue, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public string StringValue
    {
        get => _stringValue;
        set
        {
            if (SetProperty(ref _stringValue, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public bool IsNavigateToSlide { get; private set; }
    public bool IsSetVariable { get; private set; }
    public bool IsShowHideLayer { get; private set; }
    public bool IsBooleanVariable { get; private set; }
    public bool IsNumberVariable { get; private set; }
    public bool IsStringVariable { get; private set; }

    private void UpdateActionTypeVisibility()
    {
        IsNavigateToSlide = SelectedActionType == ActionType.NavigateToSlide;
        IsSetVariable = SelectedActionType == ActionType.SetVariable;
        IsShowHideLayer = SelectedActionType == ActionType.ShowLayer || SelectedActionType == ActionType.HideLayer;

        OnPropertyChanged(nameof(IsNavigateToSlide));
        OnPropertyChanged(nameof(IsSetVariable));
        OnPropertyChanged(nameof(IsShowHideLayer));

        if (IsSetVariable && SelectedVariable != null)
        {
            UpdateVariableTypeVisibility();
        }
    }

    private void UpdateVariableTypeVisibility()
    {
        if (SelectedVariable == null)
        {
            IsBooleanVariable = false;
            IsNumberVariable = false;
            IsStringVariable = false;
        }
        else
        {
            IsBooleanVariable = SelectedVariable.Type == VariableType.Boolean;
            IsNumberVariable = SelectedVariable.Type == VariableType.Number;
            IsStringVariable = SelectedVariable.Type == VariableType.String;
        }

        OnPropertyChanged(nameof(IsBooleanVariable));
        OnPropertyChanged(nameof(IsNumberVariable));
        OnPropertyChanged(nameof(IsStringVariable));
    }

    public bool IsValid
    {
        get
        {
            return SelectedActionType switch
            {
                ActionType.NavigateToSlide => SelectedSlide != null,
                ActionType.SetVariable => SelectedVariable != null && (
                    (SelectedVariable.Type == VariableType.Boolean) ||
                    (SelectedVariable.Type == VariableType.Number) ||
                    (SelectedVariable.Type == VariableType.String)
                ),
                ActionType.ShowLayer => SelectedLayer != null,
                ActionType.HideLayer => SelectedLayer != null,
                _ => false
            };
        }
    }

    // Commands will be set by the dialog to handle closing
    public IRelayCommand? OkCommand { get; set; }
    public IRelayCommand? CancelCommand { get; set; }

    public Authoring.Core.Models.Action? BuildAction()
    {
        if (!IsValid) return null;

        return SelectedActionType switch
        {
            ActionType.NavigateToSlide => new NavigateToSlideAction
            {
                TargetSlideId = SelectedSlide?.Id ?? string.Empty
            },
            ActionType.SetVariable => new SetVariableAction
            {
                VariableId = SelectedVariable?.Id ?? string.Empty,
                Value = SelectedVariable?.Type switch
                {
                    VariableType.Boolean => BooleanValue,
                    VariableType.Number => NumberValue,
                    VariableType.String => StringValue,
                    _ => null
                }
            },
            ActionType.ShowLayer => new ShowLayerAction
            {
                LayerId = SelectedLayer?.Id ?? string.Empty
            },
            ActionType.HideLayer => new HideLayerAction
            {
                LayerId = SelectedLayer?.Id ?? string.Empty
            },
            _ => null
        };
    }
}
