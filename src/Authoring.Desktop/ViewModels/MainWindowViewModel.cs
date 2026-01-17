using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Authoring.Core.Models;
using Authoring.Core.Validation;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    private readonly ISlideManagementService _slideService;
    private readonly IObjectManagementService _objectService;
    private readonly Window? _mainWindow;
    private Project? _currentProject;
    private Slide? _currentSlide;
    private SlideObject? _selectedObject;
    private EditorTool _selectedTool = EditorTool.None;
    private string? _projectFilePath;

    public MainWindowViewModel(IProjectService projectService, Window? mainWindow = null)
        : this(projectService, new SlideManagementService(), new ObjectManagementService(), mainWindow)
    {
    }

    internal MainWindowViewModel(IProjectService projectService, ISlideManagementService slideService, IObjectManagementService objectService, Window? mainWindow = null)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _slideService = slideService ?? throw new ArgumentNullException(nameof(slideService));
        _objectService = objectService ?? throw new ArgumentNullException(nameof(objectService));
        _mainWindow = mainWindow;
        Slides = new ObservableCollection<Slide>();
        Layers = new ObservableCollection<Layer>();
        Variables = new ObservableCollection<Variable>();
        SelectedObjectTriggers = new ObservableCollection<Trigger>();
        ValidationWarnings = new ObservableCollection<string>();
    }

    /// <summary>
    /// Gets or sets the current project.
    /// </summary>
    public Project? CurrentProject
    {
        get => _currentProject;
        set
        {
            if (SetProperty(ref _currentProject, value))
            {
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(HasProject));
                // Notify commands that depend on HasProject
                SaveProjectCommand.NotifyCanExecuteChanged();
                SaveAsProjectCommand.NotifyCanExecuteChanged();
                AddSlideCommand.NotifyCanExecuteChanged();
                DeleteSlideCommand.NotifyCanExecuteChanged();
                DuplicateSlideCommand.NotifyCanExecuteChanged();
                AddLayerCommand.NotifyCanExecuteChanged();
                DeleteLayerCommand.NotifyCanExecuteChanged();
                ToggleObjectTimelineCommand.NotifyCanExecuteChanged();
                DeleteSelectedObjectCommand.NotifyCanExecuteChanged();
                AddVariableCommand.NotifyCanExecuteChanged();
                EditVariableCommand.NotifyCanExecuteChanged();
                DeleteVariableCommand.NotifyCanExecuteChanged();
                AddTriggerCommand.NotifyCanExecuteChanged();
                EditTriggerCommand.NotifyCanExecuteChanged();
                DeleteTriggerCommand.NotifyCanExecuteChanged();
                UpdateSlidesCollection();
                UpdateVariablesCollection();
                ValidateProject();
                if (_currentProject?.Slides.Count > 0)
                {
                    CurrentSlide = _currentProject.Slides[0];
                }
                else
                {
                    CurrentSlide = null;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the current slide being edited.
    /// </summary>
    public Slide? CurrentSlide
    {
        get => _currentSlide;
        set
        {
            if (SetProperty(ref _currentSlide, value))
            {
                UpdateLayersCollection();
                SelectedObject = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the currently selected object on the canvas.
    /// </summary>
    public SlideObject? SelectedObject
    {
        get => _selectedObject;
        set
        {
            if (SetProperty(ref _selectedObject, value))
            {
                OnPropertyChanged(nameof(SelectedTextObject));
                OnPropertyChanged(nameof(SelectedImageObject));
                OnPropertyChanged(nameof(SelectedButtonObject));
                OnPropertyChanged(nameof(HasSelectedObject));
                OnPropertyChanged(nameof(SelectedObjectHasTimeline));
                OnPropertyChanged(nameof(SelectedObjectTimelineStartTime));
                OnPropertyChanged(nameof(SelectedObjectTimelineDuration));
                CutCommand.NotifyCanExecuteChanged();
                CopyCommand.NotifyCanExecuteChanged();
                UpdateSelectedObjectTriggers();
            }
        }
    }

    /// <summary>
    /// Marks the project as modified when object properties change.
    /// </summary>
    public void OnObjectPropertyChanged()
    {
        MarkModified();
    }

    /// <summary>
    /// Gets or sets the currently selected editor tool.
    /// </summary>
    public EditorTool SelectedTool
    {
        get => _selectedTool;
        set => SetProperty(ref _selectedTool, value);
    }

    /// <summary>
    /// Gets or sets the current project file path.
    /// </summary>
    public string? ProjectFilePath
    {
        get => _projectFilePath;
        set
        {
            if (SetProperty(ref _projectFilePath, value))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }
        }
    }

    /// <summary>
    /// Gets the window title with project name and unsaved indicator.
    /// </summary>
    public string WindowTitle
    {
        get
        {
            if (CurrentProject == null)
                return "SlideForge";
            
            var name = CurrentProject.Name;
            var unsaved = IsModified ? " *" : "";
            return $"{name}{unsaved} - SlideForge";
        }
    }

    /// <summary>
    /// Gets whether a project is currently loaded.
    /// </summary>
    public bool HasProject => CurrentProject != null;

    /// <summary>
    /// Gets whether the current project has unsaved changes.
    /// </summary>
    public bool IsModified { get; private set; }

    /// <summary>
    /// Gets the collection of slides in the current project.
    /// </summary>
    public ObservableCollection<Slide> Slides { get; }

    /// <summary>
    /// Gets the collection of layers in the current slide.
    /// </summary>
    public ObservableCollection<Layer> Layers { get; }

    /// <summary>
    /// Gets the collection of variables in the current project.
    /// </summary>
    public ObservableCollection<Variable> Variables { get; }

    /// <summary>
    /// Gets the collection of triggers for the selected object.
    /// </summary>
    public ObservableCollection<Trigger> SelectedObjectTriggers { get; }

    /// <summary>
    /// Gets the collection of validation warnings for the current project.
    /// </summary>
    public ObservableCollection<string> ValidationWarnings { get; }

    /// <summary>
    /// Gets whether there are validation warnings.
    /// </summary>
    public bool HasValidationWarnings => ValidationWarnings.Count > 0;

    /// <summary>
    /// Gets the selected object as a TextObject, or null if not a TextObject.
    /// </summary>
    public TextObject? SelectedTextObject => SelectedObject as TextObject;

    /// <summary>
    /// Gets the selected object as an ImageObject, or null if not an ImageObject.
    /// </summary>
    public ImageObject? SelectedImageObject => SelectedObject as ImageObject;

    /// <summary>
    /// Gets the selected object as a ButtonObject, or null if not a ButtonObject.
    /// </summary>
    public ButtonObject? SelectedButtonObject => SelectedObject as ButtonObject;

    [RelayCommand]
    private async Task NewProjectAsync()
    {
        var dialog = new NewProjectDialog
        {
            DataContext = new NewProjectDialogViewModel()
        };

        if (_mainWindow != null)
        {
            var result = await dialog.ShowDialog<NewProjectDialogViewModel?>(_mainWindow);
            if (result != null && dialog.ViewModel.IsValid)
            {
                var project = _projectService.CreateNewProject(dialog.ViewModel.ProjectName, dialog.ViewModel.Author);
                CurrentProject = project; // ValidateProject will be called in CurrentProject setter
                ProjectFilePath = null;
                IsModified = false;
            }
        }
        else
        {
            // Fallback if window not available
            var project = _projectService.CreateNewProject("New Project");
            CurrentProject = project;
            ProjectFilePath = null;
            IsModified = false;
        }
    }

    [RelayCommand]
    private async Task OpenProjectAsync()
    {
        if (_mainWindow == null) return;

        var topLevel = TopLevel.GetTopLevel(_mainWindow);
        if (topLevel == null) return;

        var options = new FilePickerOpenOptions
        {
            Title = "Open Project",
            FileTypeFilter = new[]
            {
                new FilePickerFileType("SlideForge Project")
                {
                    Patterns = new[] { "*.json", "*.sfproj" }
                },
                FilePickerFileTypes.All
            },
            AllowMultiple = false
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        if (files.Count > 0 && files[0].TryGetLocalPath() is { } filePath)
        {
            var project = await _projectService.OpenProjectAsync(filePath);
            if (project != null)
            {
                CurrentProject = project;
                ProjectFilePath = filePath;
                IsModified = false;
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private async Task SaveProjectAsync()
    {
        if (CurrentProject == null) return;

        if (string.IsNullOrEmpty(ProjectFilePath))
        {
            await SaveAsProjectAsync();
        }
        else
        {
            await _projectService.SaveProjectAsync(CurrentProject, ProjectFilePath);
            IsModified = false;
            OnPropertyChanged(nameof(WindowTitle));
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private async Task SaveAsProjectAsync()
    {
        if (CurrentProject == null || _mainWindow == null) return;

        var topLevel = TopLevel.GetTopLevel(_mainWindow);
        if (topLevel == null) return;

        var options = new FilePickerSaveOptions
        {
            Title = "Save Project As",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("SlideForge Project")
                {
                    Patterns = new[] { "*.json", "*.sfproj" }
                },
                FilePickerFileTypes.All
            },
            DefaultExtension = "json"
        };

        if (!string.IsNullOrEmpty(ProjectFilePath))
        {
            options.SuggestedFileName = Path.GetFileName(ProjectFilePath);
        }

        var result = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        if (result != null && result.TryGetLocalPath() is { } filePath)
        {
            await _projectService.SaveProjectAsync(CurrentProject, filePath);
            ProjectFilePath = filePath;
            IsModified = false;
            OnPropertyChanged(nameof(WindowTitle));
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void AddSlide()
    {
        if (CurrentProject == null) return;

        var newSlide = _slideService.CreateSlide(CurrentProject);
        UpdateSlidesCollection();
        CurrentSlide = newSlide;
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void DeleteSlide()
    {
        if (CurrentProject == null || CurrentSlide == null) return;

        var slideToDelete = CurrentSlide;
        var nextSlide = _slideService.DeleteSlide(CurrentProject, slideToDelete);
        
        UpdateSlidesCollection();
        ValidateProject();
        
        CurrentSlide = nextSlide;
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void DuplicateSlide()
    {
        if (CurrentProject == null || CurrentSlide == null) return;

        var newSlide = _slideService.DuplicateSlide(CurrentProject, CurrentSlide);
        UpdateSlidesCollection();
        CurrentSlide = newSlide;
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void AddLayer()
    {
        if (CurrentSlide == null) return;

        var newLayer = new Layer
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"Layer {CurrentSlide.Layers.Count + 1}",
            Visible = true
        };

        CurrentSlide.Layers.Add(newLayer);
        UpdateLayersCollection();
        ValidateProject();
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void DeleteLayer(object? parameter)
    {
        if (parameter is not Layer layer || CurrentSlide == null) return;

        // Don't allow deleting the last layer
        if (CurrentSlide.Layers.Count <= 1) return;

        CurrentSlide.Layers.Remove(layer);
        UpdateLayersCollection();
        ValidateProject();
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private async Task AddVariableAsync()
    {
        var dialog = new VariableDialog
        {
            DataContext = new VariableDialogViewModel()
        };

        if (_mainWindow != null)
        {
            var result = await dialog.ShowDialog<VariableDialogViewModel?>(_mainWindow);
            if (result != null && result.IsValid && CurrentProject != null)
            {
                var variable = new Variable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = result.Name,
                    Type = result.Type,
                    DefaultValue = result.DefaultValue
                };

                CurrentProject.AddVariable(variable);
                UpdateVariablesCollection();
                ValidateProject();
                MarkModified();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private async Task EditVariableAsync(object? parameter)
    {
        if (parameter is not Variable variable || CurrentProject == null) return;

        var dialog = new VariableDialog
        {
            DataContext = new VariableDialogViewModel(variable)
        };

        if (_mainWindow != null)
        {
            var result = await dialog.ShowDialog<VariableDialogViewModel?>(_mainWindow);
            if (result != null && result.IsValid)
            {
                variable.Name = result.Name;
                variable.Type = result.Type;
                variable.DefaultValue = result.DefaultValue;
                UpdateVariablesCollection();
                ValidateProject();
                MarkModified();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public void DeleteVariable(object? parameter)
    {
        if (parameter is not Variable variable || CurrentProject == null) return;

        CurrentProject.Variables.Remove(variable);
        UpdateVariablesCollection();
        ValidateProject();
        MarkModified();
    }

    [RelayCommand]
    private void SelectTool(object? parameter)
    {
        if (parameter is EditorTool tool)
        {
            SelectedTool = tool;
        }
        else if (parameter is string toolName)
        {
            if (Enum.TryParse<EditorTool>(toolName, out var parsedTool))
            {
                SelectedTool = parsedTool;
            }
        }
    }

    /// <summary>
    /// Gets whether the selected object has a timeline.
    /// </summary>
    public bool SelectedObjectHasTimeline => SelectedObject?.Timeline != null;

    /// <summary>
    /// Gets the selected object's timeline start time, or 0 if no timeline.
    /// </summary>
    public double SelectedObjectTimelineStartTime
    {
        get => SelectedObject?.Timeline?.StartTime ?? 0;
        set
        {
            if (SelectedObject?.Timeline != null && SelectedObject.Timeline.StartTime != value)
            {
                SelectedObject.Timeline.StartTime = value;
                OnPropertyChanged();
                MarkModified();
            }
        }
    }

    /// <summary>
    /// Gets the selected object's timeline duration, or 5.0 if no timeline.
    /// </summary>
    public double SelectedObjectTimelineDuration
    {
        get => SelectedObject?.Timeline?.Duration ?? 5.0;
        set
        {
            if (SelectedObject?.Timeline != null && SelectedObject.Timeline.Duration != value)
            {
                SelectedObject.Timeline.Duration = value;
                OnPropertyChanged();
                MarkModified();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void ToggleObjectTimeline()
    {
        if (SelectedObject == null) return;

        if (SelectedObject.Timeline == null)
        {
            SelectedObject.Timeline = new Timeline { StartTime = 0, Duration = 5.0 };
        }
        else
        {
            SelectedObject.Timeline = null;
        }

        // Notify UI that timeline properties changed
        OnPropertyChanged(nameof(SelectedObject));
        OnPropertyChanged(nameof(SelectedObjectHasTimeline));
        OnPropertyChanged(nameof(SelectedObjectTimelineStartTime));
        OnPropertyChanged(nameof(SelectedObjectTimelineDuration));
        MarkModified();
    }

    /// <summary>
    /// Creates a new object on the canvas at the specified position.
    /// </summary>
    public void CreateObjectAtPosition(double x, double y)
    {
        if (CurrentSlide == null || SelectedTool == EditorTool.None) return;

        var newObject = _objectService.CreateObject(CurrentSlide, SelectedTool, x, y);
        if (newObject != null)
        {
            SelectedObject = newObject;
            SelectedTool = EditorTool.None; // Reset tool after creation
            MarkModified();
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public void DeleteSelectedObject()
    {
        if (SelectedObject == null || CurrentSlide == null) return;

        _objectService.DeleteObject(CurrentSlide, SelectedObject);
        SelectedObject = null;
        UpdateSelectedObjectTriggers();
        ValidateProject();
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public async Task AddTriggerAsync()
    {
        if (SelectedObject == null) return;

        var dialog = new TriggerDialog
        {
            DataContext = new TriggerDialogViewModel(SelectedObject, CurrentProject, CurrentSlide)
        };

        if (_mainWindow != null)
        {
            var result = await dialog.ShowDialog<TriggerDialogViewModel?>(_mainWindow);
            if (result != null && result.IsValid)
            {
                var trigger = new Trigger
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = result.TriggerType,
                    Actions = result.Actions.ToList()
                };

                SelectedObject.Triggers.Add(trigger);
                UpdateSelectedObjectTriggers();
                ValidateProject();
                MarkModified();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public async Task EditTriggerAsync(object? parameter)
    {
        if (parameter is not Trigger trigger || SelectedObject == null) return;

        var dialog = new TriggerDialog
        {
            DataContext = new TriggerDialogViewModel(SelectedObject, CurrentProject, CurrentSlide, trigger)
        };

        if (_mainWindow != null)
        {
            var result = await dialog.ShowDialog<TriggerDialogViewModel?>(_mainWindow);
            if (result != null && result.IsValid)
            {
                trigger.Type = result.TriggerType;
                trigger.Actions.Clear();
                trigger.Actions.AddRange(result.Actions);
                UpdateSelectedObjectTriggers();
                ValidateProject();
                MarkModified();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public void DeleteTrigger(object? parameter)
    {
        if (parameter is not Trigger trigger || SelectedObject == null) return;

        SelectedObject.Triggers.Remove(trigger);
        UpdateSelectedObjectTriggers();
        ValidateProject();
        MarkModified();
    }

    public void UpdateSlidesCollection()
    {
        Slides.Clear();
        if (CurrentProject != null)
        {
            foreach (var slide in CurrentProject.Slides)
            {
                Slides.Add(slide);
            }
        }
    }

    private void UpdateLayersCollection()
    {
        Layers.Clear();
        if (CurrentSlide != null)
        {
            foreach (var layer in CurrentSlide.Layers)
            {
                Layers.Add(layer);
            }
        }
    }

    public void UpdateVariablesCollection()
    {
        Variables.Clear();
        if (CurrentProject != null)
        {
            foreach (var variable in CurrentProject.Variables)
            {
                Variables.Add(variable);
            }
        }
    }

    public void UpdateSelectedObjectTriggers()
    {
        SelectedObjectTriggers.Clear();
        if (SelectedObject != null)
        {
            foreach (var trigger in SelectedObject.Triggers)
            {
                SelectedObjectTriggers.Add(trigger);
            }
        }
        OnPropertyChanged(nameof(SelectedObjectTriggers));
    }

    public void ValidateProject()
    {
        ValidationWarnings.Clear();
        if (CurrentProject != null)
        {
            var errors = ProjectValidator.ValidateProject(CurrentProject);
            foreach (var error in errors)
            {
                ValidationWarnings.Add(error);
            }
        }
        OnPropertyChanged(nameof(HasValidationWarnings));
    }

    private void MarkModified()
    {
        IsModified = true;
        OnPropertyChanged(nameof(WindowTitle));
    }


    // Edit Menu Commands
    public bool HasSelectedObject => SelectedObject != null;
    public bool CanUndo => false; // TODO: Implement undo/redo in Phase 6
    public bool CanRedo => false; // TODO: Implement undo/redo in Phase 6
    public bool CanPaste => false; // TODO: Implement clipboard in Phase 6

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        // TODO: Implement undo functionality in Phase 6
    }

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        // TODO: Implement redo functionality in Phase 6
    }

    [RelayCommand(CanExecute = nameof(HasSelectedObject))]
    private void Cut()
    {
        if (SelectedObject == null) return;
        // TODO: Implement clipboard in Phase 6
        // For now, just delete the object (Copy() is not implemented yet)
        DeleteSelectedObject();
    }

    [RelayCommand(CanExecute = nameof(HasSelectedObject))]
    private void Copy()
    {
        if (SelectedObject == null) return;
        // TODO: Implement clipboard in Phase 6
    }

    [RelayCommand(CanExecute = nameof(CanPaste))]
    private void Paste()
    {
        // TODO: Implement clipboard in Phase 6
    }

    // View Menu Commands
    [RelayCommand]
    private void ZoomIn()
    {
        // TODO: Implement zoom in Phase 6
    }

    [RelayCommand]
    private void ZoomOut()
    {
        // TODO: Implement zoom in Phase 6
    }

    [RelayCommand]
    private void ZoomToFit()
    {
        // TODO: Implement zoom in Phase 6
    }

    [RelayCommand]
    private void ZoomTo100()
    {
        // TODO: Implement zoom in Phase 6
    }

    // Help Menu Commands
    [RelayCommand]
    private async Task AboutAsync()
    {
        if (_mainWindow == null) return;

        var dialog = new AboutDialog();
        await dialog.ShowDialog(_mainWindow);
    }

    [RelayCommand]
    private async Task DocumentationAsync()
    {
        if (_mainWindow == null) return;

        // Simple documentation window
        var docWindow = new Window
        {
            Title = "Documentation",
            Width = 500,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var content = new StackPanel { Margin = new Thickness(20), Spacing = 10 };
        content.Children.Add(new TextBlock { Text = "Documentation", FontWeight = FontWeight.Bold, Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "Documentation is available in the repository:", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "• README.md - Getting started guide", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "• ROADMAP.md - Development roadmap", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "• tests/TESTING.md - Testing guidelines", Foreground = Brushes.Black });
        
        var okButton = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
        okButton.Click += (s, e) => docWindow.Close();
        content.Children.Add(okButton);
        
        docWindow.Content = content;
        await docWindow.ShowDialog(_mainWindow);
    }

    [RelayCommand]
    private async Task KeyboardShortcutsAsync()
    {
        if (_mainWindow == null) return;

        var shortcutsWindow = new Window
        {
            Title = "Keyboard Shortcuts",
            Width = 500,
            Height = 450,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var scrollViewer = new ScrollViewer();
        var content = new StackPanel { Margin = new Thickness(20), Spacing = 8 };
        
        content.Children.Add(new TextBlock { Text = "Keyboard Shortcuts:", FontWeight = FontWeight.Bold, FontSize = 16, Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "Edit:", FontWeight = FontWeight.Bold, Margin = new Thickness(0, 10, 0, 0), Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+Z     - Undo (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+Y     - Redo (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+X     - Cut", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+C     - Copy", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+V     - Paste (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Delete     - Delete Selected Object", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "View:", FontWeight = FontWeight.Bold, Margin = new Thickness(0, 10, 0, 0), Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl++     - Zoom In (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+-     - Zoom Out (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+0     - Zoom to Fit (Coming in Phase 6)", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "File:", FontWeight = FontWeight.Bold, Margin = new Thickness(0, 10, 0, 0), Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+N     - New Project", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+O     - Open Project", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+S     - Save Project", Foreground = Brushes.Black });
        content.Children.Add(new TextBlock { Text = "  Ctrl+Shift+S - Save As", Foreground = Brushes.Black });
        
        var okButton = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
        okButton.Click += (s, e) => shortcutsWindow.Close();
        content.Children.Add(okButton);
        
        scrollViewer.Content = content;
        shortcutsWindow.Content = scrollViewer;
        await shortcutsWindow.ShowDialog(_mainWindow);
    }
}
