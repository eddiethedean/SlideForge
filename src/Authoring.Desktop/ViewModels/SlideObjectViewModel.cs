using Authoring.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Authoring.Desktop.ViewModels;

/// <summary>
/// View model wrapper for SlideObject that adds UI-specific properties.
/// </summary>
public class SlideObjectViewModel : ViewModelBase
{
    private readonly SlideObject _slideObject;
    private bool _isSelected;

    public SlideObjectViewModel(SlideObject slideObject)
    {
        _slideObject = slideObject;
    }

    /// <summary>
    /// Gets the underlying slide object.
    /// </summary>
    public SlideObject SlideObject => _slideObject;

    /// <summary>
    /// Gets or sets whether this object is currently selected on the canvas.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    /// <summary>
    /// Gets or sets the X position.
    /// </summary>
    public double X
    {
        get => _slideObject.X;
        set
        {
            if (_slideObject.X != value)
            {
                _slideObject.X = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the Y position.
    /// </summary>
    public double Y
    {
        get => _slideObject.Y;
        set
        {
            if (_slideObject.Y != value)
            {
                _slideObject.Y = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public double Width
    {
        get => _slideObject.Width;
        set
        {
            if (_slideObject.Width != value)
            {
                _slideObject.Width = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public double Height
    {
        get => _slideObject.Height;
        set
        {
            if (_slideObject.Height != value)
            {
                _slideObject.Height = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the visibility.
    /// </summary>
    public bool Visible
    {
        get => _slideObject.Visible;
        set
        {
            if (_slideObject.Visible != value)
            {
                _slideObject.Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the object type name for display.
    /// </summary>
    public string ObjectTypeName => _slideObject switch
    {
        TextObject => "Text",
        ImageObject => "Image",
        ButtonObject => "Button",
        _ => "Unknown"
    };
}
