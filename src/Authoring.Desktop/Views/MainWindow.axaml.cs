using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Authoring.Desktop.ViewModels;
using Authoring.Desktop.Views.Controls;

namespace Authoring.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private SlideCanvas? _canvas;

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _canvas = this.FindControl<SlideCanvas>("SlideCanvas");
        if (DataContext is MainWindowViewModel vm && _canvas != null)
        {
            _canvas.CanvasClicked += (s, point) =>
            {
                if (vm.SelectedTool != Models.EditorTool.None)
                {
                    vm.CreateObjectAtPosition(point.X, point.Y);
                    RefreshCanvas();
                }
            };

            _canvas.ObjectDeleted += (s, obj) =>
            {
                vm.DeleteSelectedObject();
                RefreshCanvas();
            };

            // Focus canvas to receive keyboard events
            _canvas.Focusable = true;
        }
    }

    private void RefreshCanvas()
    {
        _canvas?.InvalidateVisual();
    }

    private async void OnBrowseImageClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.SelectedImageObject != null)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Image",
                Filters = new System.Collections.Generic.List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Image Files", Extensions = { "png", "jpg", "jpeg", "gif", "bmp", "svg" } },
                    new FileDialogFilter { Name = "All Files", Extensions = { "*" } }
                },
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                vm.SelectedImageObject.SourcePath = result[0];
                vm.OnObjectPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.OnObjectPropertyChanged();
        }
    }
}