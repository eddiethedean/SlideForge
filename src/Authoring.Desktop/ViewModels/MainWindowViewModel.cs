using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Authoring.Core.Models;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    private readonly Window? _mainWindow;
    private Project? _currentProject;
    private Slide? _currentSlide;
    private SlideObject? _selectedObject;
    private EditorTool _selectedTool = EditorTool.None;
    private string? _projectFilePath;

    public MainWindowViewModel(IProjectService projectService, Window? mainWindow = null)
    {
        _projectService = projectService;
        _mainWindow = mainWindow;
        Slides = new ObservableCollection<Slide>();
        Layers = new ObservableCollection<Layer>();
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
                UpdateSlidesCollection();
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
                CurrentProject = project;
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

        var dialog = new OpenFileDialog
        {
            Title = "Open Project",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "SlideForge Project", Extensions = { "json", "sfproj" } },
                new FileDialogFilter { Name = "All Files", Extensions = { "*" } }
            },
            AllowMultiple = false
        };

        var result = await dialog.ShowAsync(_mainWindow);
        if (result != null && result.Length > 0)
        {
            var filePath = result[0];
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

        var dialog = new SaveFileDialog
        {
            Title = "Save Project As",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "SlideForge Project", Extensions = { "json", "sfproj" } },
                new FileDialogFilter { Name = "All Files", Extensions = { "*" } }
            },
            DefaultExtension = "json"
        };

        if (!string.IsNullOrEmpty(ProjectFilePath))
        {
            dialog.InitialFileName = Path.GetFileName(ProjectFilePath);
            dialog.Directory = Path.GetDirectoryName(ProjectFilePath);
        }

        var result = await dialog.ShowAsync(_mainWindow);
        if (result != null)
        {
            await _projectService.SaveProjectAsync(CurrentProject, result);
            ProjectFilePath = result;
            IsModified = false;
            OnPropertyChanged(nameof(WindowTitle));
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void AddSlide()
    {
        if (CurrentProject == null) return;

        var newSlide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Slide {CurrentProject.Slides.Count + 1}",
            Width = 1920,
            Height = 1080
        };

        var baseLayer = new Layer
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Base Layer",
            Visible = true
        };

        newSlide.Layers.Add(baseLayer);
        CurrentProject.AddSlide(newSlide);
        UpdateSlidesCollection();
        CurrentSlide = newSlide;
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void DeleteSlide()
    {
        if (CurrentProject == null || CurrentSlide == null) return;

        var slideToDelete = CurrentSlide;
        var nextSlide = CurrentProject.Slides.FirstOrDefault(s => s != slideToDelete);
        
        CurrentProject.Slides.Remove(slideToDelete);
        UpdateSlidesCollection();
        
        CurrentSlide = nextSlide ?? CurrentProject.Slides.FirstOrDefault();
        MarkModified();
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    private void DuplicateSlide()
    {
        if (CurrentProject == null || CurrentSlide == null) return;

        var sourceSlide = CurrentSlide;
        var newSlide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"{sourceSlide.Title} (Copy)",
            Width = sourceSlide.Width,
            Height = sourceSlide.Height
        };

        foreach (var sourceLayer in sourceSlide.Layers)
        {
            var newLayer = new Layer
            {
                Id = Guid.NewGuid().ToString(),
                Name = sourceLayer.Name,
                Visible = sourceLayer.Visible
            };

            foreach (var sourceObject in sourceLayer.Objects)
            {
                var newObject = CloneObject(sourceObject);
                if (newObject != null)
                {
                    newObject.Id = Guid.NewGuid().ToString();
                    newLayer.Objects.Add(newObject);
                }
            }

            newSlide.Layers.Add(newLayer);
        }

        CurrentProject.AddSlide(newSlide);
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

        OnPropertyChanged(nameof(SelectedObject));
        MarkModified();
    }

    /// <summary>
    /// Creates a new object on the canvas at the specified position.
    /// </summary>
    public void CreateObjectAtPosition(double x, double y)
    {
        if (CurrentSlide == null || SelectedTool == EditorTool.None) return;

        var baseLayer = CurrentSlide.Layers.FirstOrDefault();
        if (baseLayer == null) return;

        SlideObject? newObject = SelectedTool switch
        {
            EditorTool.Text => new TextObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Text Object",
                X = x,
                Y = y,
                Width = 200,
                Height = 50,
                Text = "Text",
                FontSize = 16,
                Visible = true
            },
            EditorTool.Image => new ImageObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Image Object",
                X = x,
                Y = y,
                Width = 100,
                Height = 100,
                SourcePath = "",
                Visible = true
            },
            EditorTool.Button => new ButtonObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Button Object",
                X = x,
                Y = y,
                Width = 150,
                Height = 40,
                Label = "Button",
                Enabled = true,
                Visible = true
            },
            _ => null
        };

        if (newObject != null)
        {
            baseLayer.Objects.Add(newObject);
            SelectedObject = newObject;
            SelectedTool = EditorTool.None; // Reset tool after creation
            MarkModified();
        }
    }

    [RelayCommand(CanExecute = nameof(HasProject))]
    public void DeleteSelectedObject()
    {
        if (SelectedObject == null || CurrentSlide == null) return;

        var baseLayer = CurrentSlide.Layers.FirstOrDefault();
        if (baseLayer != null)
        {
            baseLayer.Objects.Remove(SelectedObject);
            SelectedObject = null;
            MarkModified();
        }
    }

    private void UpdateSlidesCollection()
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

    private void MarkModified()
    {
        IsModified = true;
        OnPropertyChanged(nameof(WindowTitle));
    }

    private SlideObject? CloneObject(SlideObject source)
    {
        return source switch
        {
            TextObject text => new TextObject
            {
                Name = text.Name,
                X = text.X,
                Y = text.Y,
                Width = text.Width,
                Height = text.Height,
                Visible = text.Visible,
                Text = text.Text,
                FontFamily = text.FontFamily,
                FontSize = text.FontSize,
                Color = text.Color
            },
            ImageObject image => new ImageObject
            {
                Name = image.Name,
                X = image.X,
                Y = image.Y,
                Width = image.Width,
                Height = image.Height,
                Visible = image.Visible,
                SourcePath = image.SourcePath,
                MaintainAspectRatio = image.MaintainAspectRatio
            },
            ButtonObject button => new ButtonObject
            {
                Name = button.Name,
                X = button.X,
                Y = button.Y,
                Width = button.Width,
                Height = button.Height,
                Visible = button.Visible,
                Label = button.Label,
                Enabled = button.Enabled
            },
            _ => null
        };
    }
}
