using Authoring.Core.Models;
using Authoring.Desktop.Models;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service for managing objects within slides.
/// </summary>
public interface IObjectManagementService
{
    /// <summary>
    /// Creates a new object at the specified position on a slide.
    /// </summary>
    /// <param name="slide">The slide to add the object to.</param>
    /// <param name="tool">The tool type determining what object to create.</param>
    /// <param name="x">The X coordinate for the object.</param>
    /// <param name="y">The Y coordinate for the object.</param>
    /// <returns>The newly created object, or null if creation failed.</returns>
    SlideObject? CreateObject(Slide slide, EditorTool tool, double x, double y);

    /// <summary>
    /// Deletes an object from a slide.
    /// </summary>
    /// <param name="slide">The slide containing the object.</param>
    /// <param name="objectToDelete">The object to delete.</param>
    void DeleteObject(Slide slide, SlideObject objectToDelete);
}
