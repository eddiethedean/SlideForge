using System.Linq;
using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Desktop.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class MenuCommandsTests
{
    private readonly Mock<IProjectService> _mockService;
    private readonly MainWindowViewModel _viewModel;

    public MenuCommandsTests()
    {
        _mockService = new Mock<IProjectService>();
        _viewModel = new MainWindowViewModel(_mockService.Object);
    }

    [Fact]
    public void HasSelectedObject_WithNoSelection_ReturnsFalse()
    {
        // Assert
        Assert.False(_viewModel.HasSelectedObject);
    }

    [Fact]
    public void HasSelectedObject_WithSelection_ReturnsTrue()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();

        // Act
        _viewModel.SelectedObject = textObject;

        // Assert
        Assert.True(_viewModel.HasSelectedObject);
    }

    [Fact]
    public void CanUndo_AlwaysReturnsFalse_ForPhase6()
    {
        // Assert - Undo/Redo not implemented yet (Phase 6)
        Assert.False(_viewModel.CanUndo);
    }

    [Fact]
    public void CanRedo_AlwaysReturnsFalse_ForPhase6()
    {
        // Assert - Undo/Redo not implemented yet (Phase 6)
        Assert.False(_viewModel.CanRedo);
    }

    [Fact]
    public void CanPaste_AlwaysReturnsFalse_ForPhase6()
    {
        // Assert - Clipboard not implemented yet (Phase 6)
        Assert.False(_viewModel.CanPaste);
    }

    [Fact]
    public void UndoCommand_WhenCanUndoFalse_DoesNotExecute()
    {
        // Act
        var canExecute = _viewModel.UndoCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public void RedoCommand_WhenCanRedoFalse_DoesNotExecute()
    {
        // Act
        var canExecute = _viewModel.RedoCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public void CutCommand_WithoutSelection_DoesNotExecute()
    {
        // Act
        var canExecute = _viewModel.CutCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public void CutCommand_WithSelection_CanExecute()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        _viewModel.SelectedObject = button;

        // Act
        var canExecute = _viewModel.CutCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void CutCommand_WithSelection_DeletesObject()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        _viewModel.SelectedObject = textObject;

        var initialCount = project.Slides[0].Layers[0].Objects.Count;

        // Act
        _viewModel.CutCommand.Execute(null);

        // Assert
        Assert.True(initialCount > 0);
        Assert.True(project.Slides[0].Layers[0].Objects.Count < initialCount);
        Assert.Null(_viewModel.SelectedObject);
    }

    [Fact]
    public void CopyCommand_WithoutSelection_DoesNotExecute()
    {
        // Act
        var canExecute = _viewModel.CopyCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public void CopyCommand_WithSelection_CanExecute()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildImageObject("Image")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var imageObject = project.Slides[0].Layers[0].Objects.OfType<ImageObject>().First();
        _viewModel.SelectedObject = imageObject;

        // Act
        var canExecute = _viewModel.CopyCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void CopyCommand_WithSelection_ExecutesWithoutError()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        _viewModel.SelectedObject = button;

        // Act & Assert - Should not throw
        _viewModel.CopyCommand.Execute(null);
        // Clipboard not implemented yet, but command should execute gracefully
    }

    [Fact]
    public void PasteCommand_WhenCanPasteFalse_DoesNotExecute()
    {
        // Act
        var canExecute = _viewModel.PasteCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public void PasteCommand_ExecutesWithoutError()
    {
        // Act & Assert - Should not throw even though clipboard not implemented
        _viewModel.PasteCommand.Execute(null);
    }

    [Fact]
    public void ZoomInCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.ZoomInCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void ZoomInCommand_ExecutesWithoutError()
    {
        // Act & Assert - Should not throw (zoom not implemented yet)
        _viewModel.ZoomInCommand.Execute(null);
    }

    [Fact]
    public void ZoomOutCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.ZoomOutCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void ZoomOutCommand_ExecutesWithoutError()
    {
        // Act & Assert - Should not throw (zoom not implemented yet)
        _viewModel.ZoomOutCommand.Execute(null);
    }

    [Fact]
    public void ZoomToFitCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.ZoomToFitCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void ZoomToFitCommand_ExecutesWithoutError()
    {
        // Act & Assert - Should not throw (zoom not implemented yet)
        _viewModel.ZoomToFitCommand.Execute(null);
    }

    [Fact]
    public void ZoomTo100Command_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.ZoomTo100Command.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void ZoomTo100Command_ExecutesWithoutError()
    {
        // Act & Assert - Should not throw (zoom not implemented yet)
        _viewModel.ZoomTo100Command.Execute(null);
    }

    [Fact]
    public void AboutCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.AboutCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void DocumentationCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.DocumentationCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void KeyboardShortcutsCommand_AlwaysExecutes()
    {
        // Act
        var canExecute = _viewModel.KeyboardShortcutsCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void CutCommand_WhenSelectionChanges_UpdatesCanExecute()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();

        // Act & Assert - Initially disabled
        Assert.False(_viewModel.CutCommand.CanExecute(null));

        // Act - Select object
        _viewModel.SelectedObject = textObject;
        _viewModel.CutCommand.NotifyCanExecuteChanged();

        // Assert - Should be enabled
        Assert.True(_viewModel.CutCommand.CanExecute(null));

        // Act - Deselect
        _viewModel.SelectedObject = null;
        _viewModel.CutCommand.NotifyCanExecuteChanged();

        // Assert - Should be disabled again
        Assert.False(_viewModel.CutCommand.CanExecute(null));
    }

    [Fact]
    public void CopyCommand_WhenSelectionChanges_UpdatesCanExecute()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();

        // Act & Assert - Initially disabled
        Assert.False(_viewModel.CopyCommand.CanExecute(null));

        // Act - Select object
        _viewModel.SelectedObject = button;
        _viewModel.CopyCommand.NotifyCanExecuteChanged();

        // Assert - Should be enabled
        Assert.True(_viewModel.CopyCommand.CanExecute(null));

        // Act - Deselect
        _viewModel.SelectedObject = null;
        _viewModel.CopyCommand.NotifyCanExecuteChanged();

        // Assert - Should be disabled again
        Assert.False(_viewModel.CopyCommand.CanExecute(null));
    }

    [Fact]
    public void CutCommand_WithNullSelectedObject_DoesNotExecute()
    {
        // Arrange
        _viewModel.SelectedObject = null;

        // Act
        _viewModel.CutCommand.Execute(null);

        // Assert - Should not throw, but also should not do anything
        Assert.Null(_viewModel.SelectedObject);
    }

    [Fact]
    public void CopyCommand_WithNullSelectedObject_DoesNotExecute()
    {
        // Arrange
        _viewModel.SelectedObject = null;

        // Act
        _viewModel.CopyCommand.Execute(null);

        // Assert - Should not throw
        Assert.Null(_viewModel.SelectedObject);
    }

    [Fact]
    public void CutCommand_RemovesObjectFromSlide()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text1"))
                .WithObject(o => o.AtPosition(200, 200).BuildTextObject("Text2")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        var objects = project.Slides[0].Layers[0].Objects.ToList();
        var objectToCut = objects[0];

        var initialCount = objects.Count;
        _viewModel.SelectedObject = objectToCut;

        // Act
        _viewModel.CutCommand.Execute(null);

        // Assert
        Assert.Equal(initialCount - 1, project.Slides[0].Layers[0].Objects.Count);
        Assert.DoesNotContain(objectToCut, project.Slides[0].Layers[0].Objects);
    }

    [Fact]
    public void ZoomCommands_WithoutMainWindow_ExecuteWithoutError()
    {
        // Arrange - ViewModel created without main window
        var viewModel = new MainWindowViewModel(_mockService.Object);

        // Act & Assert - Should not throw
        viewModel.ZoomInCommand.Execute(null);
        viewModel.ZoomOutCommand.Execute(null);
        viewModel.ZoomToFitCommand.Execute(null);
        viewModel.ZoomTo100Command.Execute(null);
    }

    [Fact]
    public void HelpCommands_WithoutMainWindow_DoNotShowDialogs()
    {
        // Arrange - ViewModel created without main window
        var viewModel = new MainWindowViewModel(_mockService.Object);

        // Act & Assert - Should not throw, but won't show dialogs
        var aboutTask = viewModel.AboutCommand.ExecuteAsync(null);
        var docTask = viewModel.DocumentationCommand.ExecuteAsync(null);
        var shortcutsTask = viewModel.KeyboardShortcutsCommand.ExecuteAsync(null);

        // Commands should complete without error
        Assert.NotNull(aboutTask);
        Assert.NotNull(docTask);
        Assert.NotNull(shortcutsTask);
    }

    [Fact]
    public void MenuCommands_AfterProjectCreation_BehaveCorrectly()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildButtonObject("Button")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];

        // Act & Assert - Menu commands should work regardless of project state
        Assert.True(_viewModel.ZoomInCommand.CanExecute(null));
        Assert.True(_viewModel.AboutCommand.CanExecute(null));
        Assert.False(_viewModel.UndoCommand.CanExecute(null));
        Assert.False(_viewModel.RedoCommand.CanExecute(null));
        Assert.False(_viewModel.PasteCommand.CanExecute(null));
    }

    [Fact]
    public void CutAndCopyCommands_WithDifferentObjectTypes_WorkCorrectly()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text"))
                .WithObject(o => o.AtPosition(200, 200).BuildImageObject("Image"))
                .WithObject(o => o.AtPosition(300, 300).BuildButtonObject("Button")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];

        // Test TextObject
        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        _viewModel.SelectedObject = textObject;
        Assert.True(_viewModel.CutCommand.CanExecute(null));
        Assert.True(_viewModel.CopyCommand.CanExecute(null));

        // Test ImageObject
        var imageObject = project.Slides[0].Layers[0].Objects.OfType<ImageObject>().First();
        _viewModel.SelectedObject = imageObject;
        Assert.True(_viewModel.CutCommand.CanExecute(null));
        Assert.True(_viewModel.CopyCommand.CanExecute(null));

        // Test ButtonObject
        var buttonObject = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        _viewModel.SelectedObject = buttonObject;
        Assert.True(_viewModel.CutCommand.CanExecute(null));
        Assert.True(_viewModel.CopyCommand.CanExecute(null));
    }

    [Fact]
    public void CutCommand_MarksProjectAsModified()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o.AtPosition(100, 100).BuildTextObject("Text")))
            .Build();

        _viewModel.CurrentProject = project;
        _viewModel.CurrentSlide = project.Slides[0];
        
        // Reset modified state by saving (if possible) or just verify it changes
        var textObject = project.Slides[0].Layers[0].Objects.OfType<TextObject>().First();
        _viewModel.SelectedObject = textObject;

        // Act
        _viewModel.CutCommand.Execute(null);

        // Assert - Cut command should mark project as modified (deletes object)
        Assert.True(_viewModel.IsModified);
    }
}
