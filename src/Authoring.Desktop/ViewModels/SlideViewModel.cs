using System.Collections.ObjectModel;
using System.Linq;
using Authoring.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Authoring.Desktop.ViewModels;

/// <summary>
/// View model wrapper for Slide that manages object view models for the canvas.
/// </summary>
public class SlideViewModel : ViewModelBase
{
    private readonly Slide _slide;

    public SlideViewModel(Slide slide)
    {
        _slide = slide;
        ObjectViewModels = new ObservableCollection<SlideObjectViewModel>();
        UpdateObjectViewModels();
    }

    /// <summary>
    /// Gets the underlying slide.
    /// </summary>
    public Slide Slide => _slide;

    /// <summary>
    /// Gets the view models for objects in the slide.
    /// </summary>
    public ObservableCollection<SlideObjectViewModel> ObjectViewModels { get; }

    /// <summary>
    /// Updates the object view models collection from the slide's objects.
    /// </summary>
    public void UpdateObjectViewModels()
    {
        ObjectViewModels.Clear();
        
        // Get objects from the first visible layer (for MVP, use base layer)
        var baseLayer = _slide.Layers.FirstOrDefault();
        if (baseLayer != null)
        {
            foreach (var obj in baseLayer.Objects)
            {
                ObjectViewModels.Add(new SlideObjectViewModel(obj));
            }
        }
    }
}
