using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Authoring.Core.Models;
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

            _canvas.OnObjectPositionChanged += (s, obj) =>
            {
                // Notify ViewModel that object position changed (marks project as modified)
                vm.OnObjectPropertyChanged();
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
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var options = new FilePickerOpenOptions
            {
                Title = "Select Image",
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Image Files")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.svg" }
                    },
                    FilePickerFileTypes.All
                },
                AllowMultiple = false
            };

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            if (files.Count > 0 && files[0].TryGetLocalPath() is { } filePath)
            {
                vm.SelectedImageObject.SourcePath = filePath;
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

    private async void OnEditTriggerClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Trigger trigger && DataContext is MainWindowViewModel vm)
        {
            await vm.EditTriggerAsync(trigger);
        }
    }

    private void OnDeleteTriggerClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Trigger trigger && DataContext is MainWindowViewModel vm)
        {
            vm.DeleteTrigger(trigger);
        }
    }
}