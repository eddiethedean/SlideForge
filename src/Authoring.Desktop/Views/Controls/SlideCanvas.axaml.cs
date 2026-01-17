using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Authoring.Core.Models;
using System;
using System.Linq;

namespace Authoring.Desktop.Views.Controls;

public partial class SlideCanvas : UserControl
{
    private double _zoomLevel = 1.0;
    private Slide? _currentSlide;
    private SlideObject? _selectedObject;
    private Point _lastPanPosition;
    private bool _isPanning;
    private bool _isDragging;
    private Point _dragStartPosition;
    private Point _objectStartPosition;

    public SlideCanvas()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CurrentSlideProperty)
        {
            _currentSlide = change.GetNewValue<Slide?>();
            RefreshCanvas();
        }
        else if (change.Property == SelectedObjectProperty)
        {
            var newValue = change.GetNewValue<SlideObject?>();
            if (_selectedObject != newValue)
            {
                _selectedObject = newValue;
                RefreshCanvas();
                ObjectSelected?.Invoke(this, _selectedObject);
            }
        }
    }

    /// <summary>
    /// Gets or sets the current slide being displayed.
    /// </summary>
    public static readonly StyledProperty<Slide?> CurrentSlideProperty =
        AvaloniaProperty.Register<SlideCanvas, Slide?>(nameof(CurrentSlide));

    public Slide? CurrentSlide
    {
        get => GetValue(CurrentSlideProperty);
        set
        {
            SetValue(CurrentSlideProperty, value);
            _currentSlide = value;
            RefreshCanvas();
        }
    }

    /// <summary>
    /// Gets or sets the selected object.
    /// </summary>
    public static readonly StyledProperty<SlideObject?> SelectedObjectProperty =
        AvaloniaProperty.Register<SlideCanvas, SlideObject?>(nameof(SelectedObject), defaultBindingMode: BindingMode.TwoWay);

    public SlideObject? SelectedObject
    {
        get => GetValue(SelectedObjectProperty);
        set
        {
            SetValue(SelectedObjectProperty, value);
            if (_selectedObject != value)
            {
                _selectedObject = value;
                RefreshCanvas();
                ObjectSelected?.Invoke(this, _selectedObject);
            }
        }
    }

    /// <summary>
    /// Gets or sets the zoom level (1.0 = 100%, 0.5 = 50%, 2.0 = 200%).
    /// </summary>
    public double ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (value > 0 && value <= 5.0)
            {
                _zoomLevel = value;
                RefreshCanvas();
            }
        }
    }

    /// <summary>
    /// Event fired when an object is selected.
    /// </summary>
    public event EventHandler<SlideObject?>? ObjectSelected;

    /// <summary>
    /// Event fired when the canvas is clicked (no object hit).
    /// </summary>
    public event EventHandler<Point>? CanvasClicked;

    /// <summary>
    /// Converts a screen point to slide coordinates.
    /// </summary>
    public Point ScreenToSlide(Point screenPoint)
    {
        // Account for border margin (20px) and zoom
        // MainCanvas is inside a Border with 20px margin
        var canvasPoint = new Point((screenPoint.X - 20) / _zoomLevel, (screenPoint.Y - 20) / _zoomLevel);
        return canvasPoint;
    }

    /// <summary>
    /// Converts slide coordinates to screen point.
    /// </summary>
    public Point SlideToScreen(Point slidePoint)
    {
        // TODO: Implement coordinate conversion based on zoom and pan
        return new Point(slidePoint.X * _zoomLevel, slidePoint.Y * _zoomLevel);
    }

    private void RefreshCanvas()
    {
        MainCanvas.Children.Clear();

        if (_currentSlide == null) return;

        // Draw grid (optional, for future)
        
        // Draw objects from the base layer
        var baseLayer = _currentSlide.Layers.FirstOrDefault(l => l.Visible);
        if (baseLayer != null)
        {
            foreach (var obj in baseLayer.Objects)
            {
                if (!obj.Visible) continue;

                DrawObject(obj);
            }
        }

        // Draw selection indicator
        if (_selectedObject != null)
        {
            DrawSelection(_selectedObject);
        }
    }

    private void DrawObject(SlideObject obj)
    {
        var border = new Border
        {
            Background = Brushes.Transparent,
            BorderBrush = Brushes.LightBlue,
            BorderThickness = new Avalonia.Thickness(1),
            Width = obj.Width * _zoomLevel,
            Height = obj.Height * _zoomLevel
        };

        var textBlock = obj switch
        {
            TextObject textObj => new TextBlock
            {
                Text = textObj.Text,
                FontSize = textObj.FontSize * _zoomLevel,
                Foreground = new SolidColorBrush(Color.Parse(textObj.Color)),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            },
            ButtonObject btnObj => new TextBlock
            {
                Text = btnObj.Label,
                FontSize = 12 * _zoomLevel,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Background = Brushes.LightGray,
                Padding = new Avalonia.Thickness(10 * _zoomLevel)
            },
            ImageObject imgObj => new TextBlock
            {
                Text = "[Image]",
                FontSize = 10 * _zoomLevel,
                Foreground = Brushes.Gray,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            },
            _ => null
        };

        if (textBlock != null)
        {
            border.Child = textBlock;
        }

        border.Tag = obj;
        border.PointerPressed += OnObjectPointerPressed;

        Canvas.SetLeft(border, obj.X * _zoomLevel);
        Canvas.SetTop(border, obj.Y * _zoomLevel);

        MainCanvas.Children.Add(border);
    }

    private void DrawSelection(SlideObject obj)
    {
        var selectionBorder = new Border
        {
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Blue,
            BorderThickness = new Avalonia.Thickness(2),
            Width = obj.Width * _zoomLevel + 4,
            Height = obj.Height * _zoomLevel + 4
        };

        Canvas.SetLeft(selectionBorder, obj.X * _zoomLevel - 2);
        Canvas.SetTop(selectionBorder, obj.Y * _zoomLevel - 2);

        MainCanvas.Children.Add(selectionBorder);
    }

    private void OnObjectPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is SlideObject obj)
        {
            SelectedObject = obj;
            
            // Start dragging if left button
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _dragStartPosition = e.GetPosition(MainCanvas);
                _objectStartPosition = new Point(obj.X, obj.Y);
                
                // Capture pointer to continue dragging even if pointer leaves the object
                e.Pointer.Capture(border);
                e.Handled = true;
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var point = e.GetPosition(MainCanvas);
        var slidePoint = ScreenToSlide(point);
        
        // Check if clicking on canvas (not object)
        if (e.Source == MainCanvas || e.Source == this)
        {
            SelectedObject = null;
            CanvasClicked?.Invoke(this, slidePoint);
        }

        // Check for middle mouse for panning
        if (e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
        {
            _isPanning = true;
            _lastPanPosition = point;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isPanning)
        {
            // TODO: Implement panning
        }

        if (_isDragging && _selectedObject != null)
        {
            var currentPosition = e.GetPosition(MainCanvas);
            // Account for border margin (20px) when converting to slide coordinates
            var slideCurrentPos = ScreenToSlide(currentPosition);
            var slideStartPos = ScreenToSlide(_dragStartPosition);
            
            var deltaX = slideCurrentPos.X - slideStartPos.X;
            var deltaY = slideCurrentPos.Y - slideStartPos.Y;

            var newX = Math.Max(0, _objectStartPosition.X + deltaX);
            var newY = Math.Max(0, _objectStartPosition.Y + deltaY);
            
            // Don't allow dragging outside slide bounds
            if (_currentSlide != null)
            {
                newX = Math.Min(newX, _currentSlide.Width - _selectedObject.Width);
                newY = Math.Min(newY, _currentSlide.Height - _selectedObject.Height);
            }

            _selectedObject.X = newX;
            _selectedObject.Y = newY;

            RefreshCanvas();
            
            // Notify ViewModel of position change to update properties panel
            OnObjectPositionChanged?.Invoke(this, _selectedObject);
            
            // Notify binding of position change
            if (this.GetValue(SelectedObjectProperty) == _selectedObject)
            {
                this.SetValue(SelectedObjectProperty, _selectedObject);
            }
            
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        
        if (_isDragging)
        {
            // Release pointer capture
            if (e.Pointer.Captured != null)
            {
                e.Pointer.Capture(null);
            }
            _isDragging = false;
        }
        
        _isPanning = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Avalonia.Input.Key.Delete && _selectedObject != null)
        {
            ObjectDeleted?.Invoke(this, _selectedObject);
            e.Handled = true;
        }
    }

    /// <summary>
    /// Event fired when an object should be deleted.
    /// </summary>
    public event EventHandler<SlideObject>? ObjectDeleted;

    /// <summary>
    /// Event fired when an object's position changes (e.g., during dragging).
    /// </summary>
    public event EventHandler<SlideObject>? OnObjectPositionChanged;
}
