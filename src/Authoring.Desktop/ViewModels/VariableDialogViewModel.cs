using System;
using System.Collections.ObjectModel;
using System.Linq;
using Authoring.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class VariableDialogViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private VariableType _type = VariableType.Boolean;
    private bool _booleanValue;
    private double _numberValue;
    private string _stringValue = string.Empty;

    public VariableDialogViewModel()
    {
        VariableTypes = new ObservableCollection<VariableType>
        {
            VariableType.Boolean,
            VariableType.Number,
            VariableType.String
        };
    }

    public VariableDialogViewModel(Variable variable) : this()
    {
        _name = variable.Name;
        _type = variable.Type;
        _booleanValue = variable.DefaultValue is bool b && b;
        _numberValue = variable.DefaultValue is double d ? d : (variable.DefaultValue is int i ? i : 0);
        _stringValue = variable.DefaultValue as string ?? string.Empty;
    }

    public ObservableCollection<VariableType> VariableTypes { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public VariableType Type
    {
        get => _type;
        set
        {
            if (SetProperty(ref _type, value))
            {
                OnPropertyChanged(nameof(IsBooleanType));
                OnPropertyChanged(nameof(IsNumberType));
                OnPropertyChanged(nameof(IsStringType));
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public bool IsBooleanType => Type == VariableType.Boolean;
    public bool IsNumberType => Type == VariableType.Number;
    public bool IsStringType => Type == VariableType.String;

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

    public object? DefaultValue => Type switch
    {
        VariableType.Boolean => BooleanValue,
        VariableType.Number => NumberValue,
        VariableType.String => StringValue,
        _ => null
    };

    public bool IsValid => !string.IsNullOrWhiteSpace(Name);

    // Commands will be set by the dialog to handle closing
    public IRelayCommand? OkCommand { get; set; }
    public IRelayCommand? CancelCommand { get; set; }
}
